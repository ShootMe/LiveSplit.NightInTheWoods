using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.NightInTheWoods {
	public class SplitterComponent : IComponent {
		public TimerModel Model { get; set; }
		public string ComponentName { get { return "Night In The Woods Autosplitter " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3); } }
		public IDictionary<string, Action> ContextMenuControls { get { return null; } }
		private static string LOGFILE = "_NightInTheWoods.txt";
		private Dictionary<LogObject, string> currentValues = new Dictionary<LogObject, string>();
		private Dictionary<string, string> gameVars = new Dictionary<string, string>();
		private SplitterMemory mem;
		private int currentSplit = -1, lastLogCheck = 0;
		private bool hasLog = false, lastTransitionOut;
		private string lastSceneName = "Title";
		private SplitterSettings settings;
		private Thread updateLoop;
		public SplitterComponent(LiveSplitState state) {
			mem = new SplitterMemory();
			settings = new SplitterSettings();
			foreach (LogObject key in Enum.GetValues(typeof(LogObject))) {
				currentValues[key] = "";
			}

			if (state != null) {
				Model = new TimerModel() { CurrentState = state };
				Model.InitializeGameTime();
				Model.CurrentState.IsGameTimePaused = true;
				state.OnReset += OnReset;
				state.OnPause += OnPause;
				state.OnResume += OnResume;
				state.OnStart += OnStart;
				state.OnSplit += OnSplit;
				state.OnUndoSplit += OnUndoSplit;
				state.OnSkipSplit += OnSkipSplit;

				updateLoop = new Thread(UpdateLoop);
				updateLoop.IsBackground = true;
				updateLoop.Start();
			}
		}
		private void UpdateLoop() {
			while (updateLoop != null) {
				try {
					GetValues();
				} catch (Exception ex) {
					WriteLog(ex.ToString());
				}
				Thread.Sleep(8);
			}
		}
		public void GetValues() {
			if (!mem.HookProcess()) { return; }

			string scene = mem.SceneName();

			if (Model != null) {
				HandleSplits(scene);
			}

			lastSceneName = string.IsNullOrEmpty(scene) ? lastSceneName : scene;

			LogValues();
		}
		private void HandleSplits(string scene) {
			bool shouldSplit = false;

			if (currentSplit == -1) {
				bool transitionOut = mem.ScreenState() == TransitionState.Out;
				shouldSplit = mem.LastHooked.AddSeconds(5) < DateTime.Now && scene == "Title" && transitionOut && !lastTransitionOut;
				lastTransitionOut = transitionOut;
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				bool loading = mem.Loading();
				Dictionary<string, string> info = mem.Variables();
				if (currentSplit < Model.CurrentState.Run.Count && currentSplit < settings.Splits.Count) {
					SplitName split = settings.Splits[currentSplit];

					switch (split) {
						case SplitName.A1D1: shouldSplit = scene == "MaeRoom" && info["act"] == "1" && info["day"] == "1"; break;
						case SplitName.Part1: shouldSplit = scene == "SectionTitle_Part1"; break;
						case SplitName.A1D2: shouldSplit = info["act"] == "1" && info["day"] == "2"; break;
						case SplitName.A1D3: shouldSplit = info["act"] == "1" && info["day"] == "3"; break;
						case SplitName.Party: shouldSplit = scene == "SectionTitle_TheParty"; break;
						case SplitName.A1D4: shouldSplit = scene == "AstralAct1Day3" && info["act"] == "1" && info["day"] == "3"; break;
						case SplitName.Part2: shouldSplit = scene == "SectionTitle_Part2"; break;
						case SplitName.OldGods: shouldSplit = scene == "SectionTitle_BeaFQ1Intro"; break;
						case SplitName.A2D1: shouldSplit = info["act"] == "2" && info["day"] == "2"; break;
						case SplitName.A2D2: shouldSplit = info["act"] == "2" && info["day"] == "3"; break;
						case SplitName.A2D3: shouldSplit = info["act"] == "2" && info["day"] == "4"; break;
						case SplitName.A2D4: shouldSplit = info["act"] == "2" && info["day"] == "5"; break;
						case SplitName.A2D5: shouldSplit = info["act"] == "3" && info["day"] == "1"; break;
						case SplitName.A3D1: shouldSplit = info["act"] == "3" && info["day"] == "2"; break;
						case SplitName.A3D2: shouldSplit = info["act"] == "3" && info["day"] == "3"; break;
						case SplitName.A3D3: shouldSplit = info["act"] == "3" && info["day"] == "4"; break;
						case SplitName.A3D4: shouldSplit = info["act"] == "3" && info["day"] == "5"; break;
						case SplitName.A3D5: shouldSplit = info["act"] == "4" && info["day"] == "1"; break;
						case SplitName.A4D1: shouldSplit = info["act"] == "4" && info["day"] == "2"; break;
						case SplitName.A4D2: shouldSplit = info["act"] == "4" && info["day"] == "3"; break;
					}
				}

				Model.CurrentState.IsGameTimePaused = Model.CurrentState.CurrentPhase != TimerPhase.Running || loading;
			}

			HandleSplit(shouldSplit, false);
		}
		private void HandleSplit(bool shouldSplit, bool shouldReset = false) {
			if (shouldReset) {
				if (currentSplit >= 0) {
					Model.Reset();
				}
			} else if (shouldSplit) {
				if (currentSplit < 0) {
					Model.Start();
				} else {
					Model.Split();
				}
			}
		}
		private void LogValues() {
			if (lastLogCheck == 0) {
				hasLog = File.Exists(LOGFILE);
				lastLogCheck = 300;
			}
			lastLogCheck--;

			if (hasLog || !Console.IsOutputRedirected) {
				string prev = string.Empty, curr = string.Empty;
				foreach (LogObject key in Enum.GetValues(typeof(LogObject))) {
					prev = currentValues[key];

					switch (key) {
						case LogObject.CurrentSplit: curr = currentSplit.ToString(); break;
						case LogObject.SceneName: curr = lastSceneName; break;
						case LogObject.Loading: curr = mem.Loading().ToString(); break;
						case LogObject.ScreenState: curr = mem.ScreenState().ToString(); break;
						case LogObject.Info:
							Dictionary<string, string> info = mem.Variables();
							foreach (KeyValuePair<string, string> pair in info) {
								string oldVal = string.Empty;
								if (!gameVars.TryGetValue(pair.Key, out oldVal) || oldVal != pair.Value) {
									string newVal = pair.Value;
									gameVars[pair.Key] = pair.Value;
									if (string.IsNullOrEmpty(oldVal)) { oldVal = string.Empty; }
									if (string.IsNullOrEmpty(newVal)) { newVal = string.Empty; }
									int minus = (pair.Key.Length >= 16 ? pair.Key.Length - 14 : 0);
									WriteLogWithTime(pair.Key + ": ".PadRight(16 - (pair.Key.Length > 16 ? 16 : pair.Key.Length), ' ') + oldVal.PadLeft(25 - (minus > 25 ? 25 : minus), ' ') + " -> " + newVal);
								}
							}
							curr = string.Empty;
							break;
						default: curr = string.Empty; break;
					}

					if (prev == null) { prev = string.Empty; }
					if (curr == null) { curr = string.Empty; }
					if (!prev.Equals(curr)) {
						WriteLogWithTime(key.ToString() + ": ".PadRight(16 - key.ToString().Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);

						currentValues[key] = curr;
					}
				}
			}
		}
		private void WriteLog(string data) {
			lock (LOGFILE) {
				if (hasLog || !Console.IsOutputRedirected) {
					if (!Console.IsOutputRedirected) {
						Console.WriteLine(data);
					}
					if (hasLog) {
						using (StreamWriter wr = new StreamWriter(LOGFILE, true)) {
							wr.WriteLine(data);
						}
					}
				}
			}
		}
		private void WriteLogWithTime(string data) {
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null && Model.CurrentState.CurrentTime.RealTime.HasValue ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": " + data);
		}
		public void Update(IInvalidator invalidator, LiveSplitState lvstate, float width, float height, LayoutMode mode) {
		}
		public void OnReset(object sender, TimerPhase e) {
			currentSplit = -1;
			WriteLog("---------Reset----------------------------------");
		}
		public void OnResume(object sender, EventArgs e) {
			WriteLog("---------Resumed--------------------------------");
		}
		public void OnPause(object sender, EventArgs e) {
			WriteLog("---------Paused---------------------------------");
		}
		public void OnStart(object sender, EventArgs e) {
			currentSplit = 0;
			Model.CurrentState.SetGameTime(TimeSpan.Zero);
			Model.CurrentState.IsGameTimePaused = true;
			WriteLog("---------New Game " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3) + "-------------------------");
		}
		public void OnUndoSplit(object sender, EventArgs e) {
			currentSplit--;
			WriteLog("---------Undo-----------------------------------");
		}
		public void OnSkipSplit(object sender, EventArgs e) {
			currentSplit++;
			WriteLog("---------Skip-----------------------------------");
		}
		public void OnSplit(object sender, EventArgs e) {
			currentSplit++;
			WriteLog("---------Split----------------------------------");
		}
		public Control GetSettingsControl(LayoutMode mode) { return settings; }
		public void SetSettings(XmlNode document) { settings.SetSettings(document); }
		public XmlNode GetSettings(XmlDocument document) { return settings.UpdateSettings(document); }
		public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) { }
		public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) { }
		public float HorizontalWidth { get { return 0; } }
		public float MinimumHeight { get { return 0; } }
		public float MinimumWidth { get { return 0; } }
		public float PaddingBottom { get { return 0; } }
		public float PaddingLeft { get { return 0; } }
		public float PaddingRight { get { return 0; } }
		public float PaddingTop { get { return 0; } }
		public float VerticalHeight { get { return 0; } }
		public void Dispose() {
			if (updateLoop != null) {
				updateLoop = null;
			}
			if (Model != null) {
				Model.CurrentState.OnReset -= OnReset;
				Model.CurrentState.OnPause -= OnPause;
				Model.CurrentState.OnResume -= OnResume;
				Model.CurrentState.OnStart -= OnStart;
				Model.CurrentState.OnSplit -= OnSplit;
				Model.CurrentState.OnUndoSplit -= OnUndoSplit;
				Model.CurrentState.OnSkipSplit -= OnSkipSplit;
				Model = null;
			}
		}
	}
}