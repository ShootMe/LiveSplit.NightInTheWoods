﻿using System;
using System.Diagnostics;
namespace LiveSplit.NightInTheWoods {
	public enum PointerVersion {
		Steam
	}
	public enum AutoDeref {
		None,
		Single,
		Double
	}
	public enum DerefType {
		Int32,
		Int64
	}
	public class ProgramSignature {
		public PointerVersion Version { get; set; }
		public string Signature { get; set; }
		public int Offset { get; set; }
		public ProgramSignature(PointerVersion version, string signature, int offset) {
			Version = version;
			Signature = signature;
			Offset = offset;
		}
		public override string ToString() {
			return Version.ToString() + " - " + Signature;
		}
	}
	public class ProgramPointer {
		private int lastID;
		private DateTime lastTry;
		private ProgramSignature[] signatures;
		private int[] offsets;
		public IntPtr Pointer { get; private set; }
		public PointerVersion Version { get; private set; }
		public AutoDeref AutoDeref { get; private set; }
		public DerefType DerefType { get; private set; }

		public ProgramPointer(AutoDeref autoDeref, DerefType derefType, params ProgramSignature[] signatures) {
			AutoDeref = autoDeref;
			DerefType = derefType;
			this.signatures = signatures;
			lastID = -1;
			lastTry = DateTime.MinValue;
		}
		public ProgramPointer(AutoDeref autoDeref, DerefType derefType, params int[] offsets) {
			AutoDeref = autoDeref;
			DerefType = derefType;
			this.offsets = offsets;
			lastID = -1;
			lastTry = DateTime.MinValue;
		}

		public T Read<T>(Process program, params int[] offsets) where T : struct {
			GetPointer(program);
			return program.Read<T>(Pointer, offsets);
		}
		public string Read(Process program, IntPtr address) {
			GetPointer(program);
			return program.ReadString(address);
		}
		public string Read(Process program, params int[] offsets) {
			GetPointer(program);
			return program.ReadString(Pointer, offsets);
		}
		public string ReadAscii(Process program, IntPtr address) {
			GetPointer(program);
			return program.ReadAscii(address);
		}
		public byte[] ReadBytes(Process program, int length, params int[] offsets) {
			GetPointer(program);
			return program.ReadBytes(Pointer, length, offsets);
		}
		public void Write<T>(Process program, T value, params int[] offsets) where T : struct {
			GetPointer(program);
			program.Write<T>(Pointer, value, offsets);
		}
		public void Write(Process program, byte[] value, params int[] offsets) {
			GetPointer(program);
			program.Write(Pointer, value, offsets);
		}
		public void ClearPointer() {
			Pointer = IntPtr.Zero;
		}
		public IntPtr GetPointer(Process program) {
			if (program == null) {
				Pointer = IntPtr.Zero;
				lastID = -1;
				return Pointer;
			} else if (program.Id != lastID) {
				Pointer = IntPtr.Zero;
				lastID = program.Id;
			}

			if (Pointer == IntPtr.Zero && DateTime.Now > lastTry.AddSeconds(1)) {
				lastTry = DateTime.Now;

				Pointer = GetVersionedFunctionPointer(program);
				if (Pointer != IntPtr.Zero) {
					if (AutoDeref != AutoDeref.None) {
						if (DerefType == DerefType.Int32) {
							Pointer = (IntPtr)program.Read<uint>(Pointer);
						} else {
							Pointer = (IntPtr)program.Read<ulong>(Pointer);
						}
						if (AutoDeref == AutoDeref.Double) {
							if (DerefType == DerefType.Int32) {
								Pointer = (IntPtr)program.Read<uint>(Pointer);
							} else {
								Pointer = (IntPtr)program.Read<ulong>(Pointer);
							}
						}
					}
				}
			}
			return Pointer;
		}
		private IntPtr GetVersionedFunctionPointer(Process program) {
			if (signatures != null) {
				MemorySearcher searcher = new MemorySearcher();
				searcher.MemoryFilter = delegate (MemInfo info) {
					return (info.State & 0x1000) != 0 && (info.Protect & 0x40) != 0 && (info.Protect & 0x100) == 0 && (long)info.RegionSize <= 0x100000;
				};
				for (int i = 0; i < signatures.Length; i++) {
					ProgramSignature signature = signatures[i];

					IntPtr ptr = searcher.FindSignature(program, signature.Signature);
					if (ptr != IntPtr.Zero) {
						Version = signature.Version;
						return ptr + signature.Offset;
					}
				}
				return IntPtr.Zero;
			}

			if (DerefType == DerefType.Int32) {
				return (IntPtr)program.Read<uint>(program.MainModule.BaseAddress, offsets);
			} else {
				return (IntPtr)program.Read<ulong>(program.MainModule.BaseAddress, offsets);
			}
		}
	}
}