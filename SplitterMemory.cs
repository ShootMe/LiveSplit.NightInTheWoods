using System;
using System.Diagnostics;
using System.IO;
namespace LiveSplit.NightInTheWoods {
	public partial class SplitterMemory {
		private static ProgramPointer Global = new ProgramPointer(AutoDeref.Single, DerefType.Int32, new ProgramSignature(PointerVersion.Steam, "55488BEC564883EC78488BF1B8????????488930488BCE4883EC20", 13));
		private static ProgramPointer SceneManager = new ProgramPointer(AutoDeref.None, DerefType.Int32, 0x144e950);
		public Process Program { get; set; }
		public bool IsHooked { get; set; } = false;
		public DateTime LastHooked;

		public SplitterMemory() {
			LastHooked = DateTime.MinValue;
		}

		public bool Loading() {
			//Global.isLoading
			bool loading = Global.Read<bool>(Program, 0x38);
			return !IsHooked || loading || SceneManager.Read<int>(Program, 0x18) > 1 || SceneManager.Read<int>(Program, 0x38) > 0;
		}
		public string SceneName() {
			return Path.GetFileNameWithoutExtension(SceneManager.ReadAscii(Program, (IntPtr)SceneManager.Read<uint>(Program, 0x48, 0x18)));
		}
		public bool HookProcess() {
			IsHooked = Program != null && !Program.HasExited;
			if (!IsHooked && DateTime.Now > LastHooked.AddSeconds(1)) {
				LastHooked = DateTime.Now;
				Process[] processes = Process.GetProcessesByName("Night in the Woods");
				Program = processes != null && processes.Length > 0 ? processes[0] : null;

				if (Program != null && !Program.HasExited) {
					MemoryReader.Update64Bit(Program);
					IsHooked = true;
				}
			}

			return IsHooked;
		}
		public void Dispose() {
			if (Program != null) {
				Program.Dispose();
			}
		}
	}
}