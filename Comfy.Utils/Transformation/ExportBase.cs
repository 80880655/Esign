using System;
using System.Drawing;
using System.Collections;
using System.IO;
using System.Collections.Generic;
namespace Comfy.Utils.Transformation {
	public interface IExportProvider : IDisposable {
		void Commit();
		int RegisterStyle(ExportCacheCellStyle style);
		void SetDefaultStyle(ExportCacheCellStyle style);
		void SetStyle(ExportCacheCellStyle style);
		void SetStyle(int styleIndex);
		void SetCellStyle(int col, int row, int styleIndex);
		void SetCellStyle(int col, int row, ExportCacheCellStyle style);
		void SetCellStyle(int col, int row, int exampleCol, int exampleRow);
		void SetCellUnion(int col, int row, int width, int height);
		void SetCellStyleAndUnion(int col, int row, int width, int height, int styleIndex);
		void SetCellStyleAndUnion(int col, int row, int width, int height, ExportCacheCellStyle style);
		void SetRange(int width, int height, bool isVisible);
		void SetColumnWidth(int col, int width);
		void SetRowHeight(int row, int height);
		void SetCellData(int col, int row, object data);
		void SetCellString(int col, int row, string str);
		ExportCacheCellStyle GetStyle(int styleIndex);
		ExportCacheCellStyle GetCellStyle(int col, int row);
		ExportCacheCellStyle GetDefaultStyle();
		int GetColumnWidth(int col);
		int GetRowHeight(int row);
		IExportProvider Clone(string fileName, Stream stream);
		bool IsStreamMode { get; }
		Stream Stream { get; }
		event ProviderProgressEventHandler ProviderProgress;
	}
	public interface IExportInternalProvider {
		void CommitCache(StreamWriter writer);
		void SetCacheToCell(int col, int row, IExportInternalProvider cache);
		void DeleteCacheFromCell(int col, int row);
	}
	public enum ExportCacheDataType {
		Boolean,
		Integer,
		Double,
		Decimal,
		String,
		Object,
		Single
	}
	public enum BrushStyle {
		Clear,
		Solid
	}
	public struct ExportCacheItem {		
		public IExportInternalProvider InternalCache;
		public object Data;
		public ExportCacheDataType DataType;
		public int StyleIndex;
		public bool IsUnion;
		public bool IsHidden;
		public int UnionWidth;
		public int UnionHeight;
	}
	public struct ExportCacheCellStyle {
		Hashtable types;
		Hashtable Types {
			get {
				if(types == null) types = new Hashtable();
				return types;
			}
		}
		public Color TextColor;
		public Font TextFont;
		public StringAlignment TextAlignment;
		public StringAlignment LineAlignment;
		public String FormatString;
		public String Url;
		public Color BkColor;
		public Color FgColor;
		public BrushStyle BrushStyle_;
		public ExportCacheCellBorderStyle LeftBorder;
		public ExportCacheCellBorderStyle TopBorder;
		public ExportCacheCellBorderStyle RightBorder;
		public ExportCacheCellBorderStyle BottomBorder;
		public bool IsEqual(ExportCacheCellStyle style) {
			return style.BkColor == BkColor && style.FgColor == FgColor && style.TextColor == TextColor &&
				TextFont.Equals(style.TextFont) && style.BrushStyle_ == BrushStyle_ &&
				LeftBorder.IsEqual(style.LeftBorder) && TopBorder.IsEqual(style.TopBorder) &&
				RightBorder.IsEqual(style.RightBorder) && BottomBorder.IsEqual(style.BottomBorder) &&
				(style.TextAlignment == TextAlignment) && (style.LineAlignment == LineAlignment) 
				&& (style.FormatString == FormatString) && (style.Url == Url);
		}
		public override bool Equals(object obj) {
			if(obj is ExportCacheCellStyle) {
				return IsEqual((ExportCacheCellStyle)obj);
			}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		internal bool WasExportedWithType(ushort type) {
			return Types.ContainsKey(type);
		}
		internal void AddExportedType(ushort type, int result) {
			if(!WasExportedWithType(type))
				Types[type] = result;
		}
		internal int GetExportResult(ushort type) {
			return (int)Types[type];
		}
	}
	public struct ExportCacheCellBorderStyle {
		public bool IsDefault;
		public Color Color_;
		public int Width;
		public bool IsEqual(ExportCacheCellBorderStyle borderStyle) {
			return borderStyle.Width == 0 && Width == 0 ? true :
				borderStyle.IsDefault == IsDefault && borderStyle.Color_ == Color_ && borderStyle.Width == Width;
		}
		public override bool Equals(object obj) {
			if(obj is ExportCacheCellBorderStyle) {
				return IsEqual((ExportCacheCellBorderStyle)obj);
			}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public class ExportCustomProvider : IDisposable {
		private string fileName;
		private Stream stream;
		private bool isStreamMode;
		public const string StreamModeName = "Stream";
		public ExportCustomProvider(string fileName) {
			this.fileName = fileName;
			isStreamMode = false;
		}
		public ExportCustomProvider(Stream stream) {
			this.stream = stream;
			isStreamMode = true;
		}
		protected void OnProviderProgress(int position) {
			if(ProviderProgress != null)
				ProviderProgress(this, new ProviderProgressEventArgs(position));
		}
		public static bool IsValidFileName(string fileName) {
			return fileName != null && fileName != "";
		}
		public static bool IsValidStream(Stream stream) {
			return stream != null;
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				ExportStyleManager.DisposeInstance(fileName, stream);
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ExportCustomProvider() {
			Dispose(false);
		}
		#endregion
		public string FileName { get { return fileName;	} }
		public Stream Stream { get { return stream; } }
		public bool IsStreamMode { get { return isStreamMode; }}
		public event ProviderProgressEventHandler ProviderProgress;
	}
	public sealed class ExportStyleManager {
		private static Hashtable instances = new Hashtable();
		private static Hashtable instances2 = new Hashtable();
		private List<ExportCacheCellStyle> styles = new List<ExportCacheCellStyle>();
		private string fileName;
		private Stream stream;
		private ExportStyleManager() {
			this.fileName = "";
			this.stream = null;
			RegisterStyle(CreateDefaultStyle());			
		}
		private ExportStyleManager(string fileName, Stream stream) {
			this.fileName = fileName;
			this.stream = stream;
			RegisterStyle(CreateDefaultStyle());			
		}
		private ExportCacheCellStyle CreateDefaultStyle() {
				ExportCacheCellStyle result = new ExportCacheCellStyle();
				result.TextColor = SystemColors.WindowText;
				result.TextAlignment = StringAlignment.Near;
				result.LineAlignment = StringAlignment.Near;
				result.TextFont = new Font("Tahoma", 8);
				result.BkColor = SystemColors.Window;
				result.FgColor = Color.Black;
				result.BrushStyle_ = BrushStyle.Solid;
				result.LeftBorder.Color_ = SystemColors.ActiveBorder;
				result.TopBorder.Color_ = SystemColors.ActiveBorder;
				result.RightBorder.Color_ = SystemColors.ActiveBorder;
				result.BottomBorder.Color_ = SystemColors.ActiveBorder;
				result.LeftBorder.Width = 1;
				result.TopBorder.Width = 1;
				result.RightBorder.Width = 1;
				result.BottomBorder.Width = 1;
				return result;
			}
		private static ExportStyleManager GetFileInstance(string fileName) {
			ExportStyleManager result = (ExportStyleManager)instances[fileName];
			if(result == null) { 
				result = new ExportStyleManager(fileName, null);
				instances.Add(fileName, result);
			}
			return result;
		}
		private static ExportStyleManager GetStreamInstance(Stream stream) {
			ExportStyleManager result = (ExportStyleManager)instances2[stream];
			if(result == null) { 
				result = new ExportStyleManager("", stream);
				instances2.Add(stream, result);
			}
			return result;
		}
		public int RegisterStyle(ExportCacheCellStyle style) {
			for(int i = 0; i < Count; i++)
				if(this[i].IsEqual(style))
					return i;
			styles.Add(style);
			return styles.Count - 1;
		}
		public void Clear() {
			styles.Clear();
			RegisterStyle(CreateDefaultStyle());
		}
		internal void MarkStyleAsExported(int styleIndex, int result, ushort type) {
			ExportCacheCellStyle style = this[styleIndex];
			style.AddExportedType(type, result);
			styles[styleIndex] = style;
		}
		public static void DisposeInstance(string fileName, Stream stream) {
			if(ExportCustomProvider.IsValidFileName(fileName))
				instances.Remove(fileName);
			else {
				if(ExportCustomProvider.IsValidStream(stream))
					instances2.Remove(stream);
				else
					throw new ExportCacheException("Can't dispose the instance of ExportStyleManager class: Ivalid parameter values.");
			}
		}
		public static ExportStyleManager GetInstance(string fileName, Stream stream) {
			if(ExportCustomProvider.IsValidFileName(fileName))
				return GetFileInstance(fileName);
			else {
				if(ExportCustomProvider.IsValidStream(stream))
					return GetStreamInstance(stream);
				else
					throw new ExportCacheException("Can't create the instance of ExportStyleManager class: Ivalid parameter values.");
			}
		}
		public int Count {
				get {
					return styles.Count;
				}
			}
		public ExportCacheCellStyle this[int index] {
				get {
					return styles[index];
				}
			}
		public ExportCacheCellStyle DefaultStyle {
				get {
					return styles[0];
				}
				set {
					if(!styles[0].IsEqual(value))
						styles[0] = value;
				}
			}
	}
	public class ExportCacheException: ApplicationException {
		public ExportCacheException(string message): base(message) {
			}
	}
	public delegate void ProviderProgressEventHandler(object sender, ProviderProgressEventArgs e);
	public class ProviderProgressEventArgs : EventArgs {
		int position;
		public ProviderProgressEventArgs(int position) : base() {
			this.position = position;
		}
		public int Position { get { return position; } }
	}
}
