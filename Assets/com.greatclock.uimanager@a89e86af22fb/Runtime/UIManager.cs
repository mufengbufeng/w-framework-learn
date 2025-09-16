using System;
using System.Collections.Generic;
using UnityEngine;

namespace GreatClock.Common.UI {

	public static partial class UIManager {

		public static UIRoot Root { get; private set; }

		public static void Init(IUILoader uiLoader, IUILoadingOverlay loadingOverlay, bool useLogicCache) {
			if (uiLoader == null) { throw new ArgumentNullException(nameof(uiLoader)); }
			if (s_uiloader != null) { throw new InvalidOperationException(); }
			s_uiloader = uiLoader;
			s_loading_overlay = loadingOverlay;
			if (useLogicCache) {
				s_logic_cache = new Dictionary<Type, Queue<IUILogicBase>>();
				s_logic_to_type = new Dictionary<object, Type>();
			}
			TryInit();
		}

		public static UIManagerExtend ex { get; private set; } = new UIManagerExtend();

		public static bool Open(string id) {
			return OpenInternal(id, null, null);
		}

		public static bool Open(string id, object parameter) {
			return OpenInternal(id, parameter, null);
		}

		public static bool OpenWithHandler(IUIEventHandler handler, string id) {
			return OpenInternal(id, null, handler);
		}

		public static bool OpenWithHandler(IUIEventHandler handler, string id, object parameter) {
			return OpenInternal(id, parameter, handler);
		}

		public static bool Open(ParametersForUI cfg) {
			return OpenInternal(cfg, null, null);
		}

		public static bool Open(ParametersForUI cfg, object parameter) {
			return OpenInternal(cfg, parameter, null);
		}

		public static bool OpenWithHandler(IUIEventHandler handler, ParametersForUI cfg) {
			return OpenInternal(cfg, null, handler);
		}

		public static bool OpenWithHandler(IUIEventHandler handler, ParametersForUI cfg, object parameter) {
			return OpenInternal(cfg, parameter, handler);
		}

		public static bool CloseSingle(string id) {
			if (s_processor == null) { return false; }
			return s_processor.CloseSingle(id);
		}

		public static bool CloseGroup(string id) {
			if (s_processor == null) { return false; }
			return s_processor.CloseGroup(id);
		}

		public static bool CloseSingle(IUILogicBase logic) {
			if (logic == null) { return false; }
			if (s_processor == null) { return false; }
			return s_processor.CloseSingle(logic);
		}

		public static bool CloseGroup(IUILogicBase logic) {
			if (logic == null) { return false; }
			if (s_processor == null) { return false; }
			return s_processor.CloseGroup(logic);
		}

		public static void SetUIRoot(UIRoot root) {
			if (Root != null) {
				throw new InvalidOperationException();
			}
			if (root == null) {
				throw new ArgumentNullException(nameof(root));
			}
			Root = root;
			TryInit();
		}

		private static Processor s_processor;

		private static void TryInit() {
			if (Root == null || s_uiloader == null) { return; }
			if (s_processor != null) { return; }
			InitCameraRange();
			s_processor = new Processor();
		}

		private static bool OpenInternal(string id, object parameter, IUIEventHandler handler) {
			if (s_processor == null) { return false; }
			ParametersForUI cfg = s_uiloader.GetParameterForUI(id);
			if (cfg.id != id) { return false; }
			return s_processor.Open(cfg, parameter, handler);
		}

		private static bool OpenInternal(ParametersForUI cfg, object parameter, IUIEventHandler handler) {
			if (s_processor == null) { return false; }
			if (string.IsNullOrEmpty(cfg.id)) { return false; }
			return s_processor.Open(cfg, parameter, handler);
		}

		#region update dispatch

		public static void Update() {
			if (s_processor == null) { return; }
			CheckScreenOrCameraChanged();
			if (Input.GetKeyDown(KeyCode.Escape)) {
				try { s_processor.RrocessESC(); } catch (Exception e) { Debug.LogException(e); }
			}
		}

		#endregion

		private static IUILoader s_uiloader;
		private static IUILoadingOverlay s_loading_overlay;

	}

}
