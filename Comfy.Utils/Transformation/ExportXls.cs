

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;
namespace Comfy.Utils.Transformation {
	public class ExportXlsProvider: ExportCustomProvider, IExportProvider {
		private int[] palette = new int[56];
		private int usedColors;
		private XlsRecordList fonts = new XlsRecordList(XlsConsts.Font);
		private XlsRecordList styles = new XlsRecordList(XlsConsts.XF);
		private XlsCellData cells = new XlsCellData();
		private DynamicMergeRectBuffer unionCells = new DynamicMergeRectBuffer();
		private XlsRecordList colStyles = new XlsRecordList(XlsConsts.COLINFO);
		private XlsRecordList rowStyles = new XlsRecordList(XlsConsts.Row);
		private ExportStyleManager styleCache;
		private int maxCol = -1;
		private int maxRow = -1;
		private int unionCellsCount;				
		private int unionCellsCapacity;
		private XlsWorkBookWriter workBoolWriter = new XlsWorkBookWriter();
		private XlsStringTable sst = new XlsStringTable();
		private XlsStream stream;
		byte[] unicodeSheetNameBytes;
		private bool visibleGrid;
		protected string invalidCellDimension = "Invalid cell dimension";
		XlsPictureCollection pictures = new XlsPictureCollection(); 
		XlsPictureWriter picturesWriter = new XlsPictureWriter();
		List<XlsHyperlink> hyperlinks = new List<XlsHyperlink>();
		#region tests
#if DEBUGTEST
		internal DynamicMergeRectBuffer UnionCells { get { return unionCells; } 
		}
#endif
		#endregion
		public ExportXlsProvider(string fileName): base(fileName) {
			InitializeSheetName(string.Empty, Path.GetFileNameWithoutExtension(fileName));
			this.stream = CreateXlsStream(new FileStream(fileName, FileMode.Create, FileAccess.Write));
			styleCache = ExportStyleManager.GetInstance(fileName, null);
			Initialize();
		}
		public ExportXlsProvider(Stream stream, string sheetName)
			: base(stream) {
			InitializeSheetName(sheetName, (stream is FileStream) ? Path.GetFileNameWithoutExtension(((FileStream)stream).Name) : ExportCustomProvider.StreamModeName);
			this.stream = CreateXlsStream(stream);
			styleCache = ExportStyleManager.GetInstance("", stream);
			Initialize();
		}
		public ExportXlsProvider(Stream stream)
			: this(stream, string.Empty) {
		}
		void InitializeSheetName(string sheetName, string defaultSheetName) {
			if(string.IsNullOrEmpty(sheetName))
				sheetName = defaultSheetName;
			this.unicodeSheetNameBytes = System.Text.Encoding.Unicode.GetBytes(sheetName);
		}
		protected internal XlsPictureCollection Pictures { get { return pictures; } }
		protected internal XlsCellData Cells { get { return cells; } }
		protected XlsStream CreateXlsStream(Stream stream) {
			return new XlsStream(stream);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				pictures.Clear();
				pictures = null;
				picturesWriter.Dispose();
			}
			base.Dispose(disposing);
		}
		void Initialize() {
			usedColors = palette.Length - 1;
			MoveXlsPalette();
		}
		string ValidateString(string str) {
			string result = str;
			int i = 0;
			while(i < result.Length) {
				if(result[i] == '\x000D')
					result = result.Remove(i, 1);
				else
					i++;
			}
			return result;
		}
		private void MoveXlsPalette() {
			for(int i = 0; i < XlsConsts.SizeOfPalette_ / 4; i++)
				palette[i] = XlsConsts.Palette_[i];
		}
		private byte[] StringToByteArray(string str) {
			byte[] result = new byte[str.Length * 2];
			int index = 0;
			for(int i = 0; i < str.Length; i++) {
				byte[] ch = BitConverter.GetBytes(str[i]);
				result[index] = ch[0];
				result[index + 1] = ch[1];
				index += 2;
			}
			return result;
		}
		private ExportCacheCellBorderStyle GetBorderStyle(ExportCacheCellStyle style, int index) {
			switch(index) {
				case 0:
					return style.LeftBorder;
				case 1:
					return style.RightBorder;
				case 2:
					return style.TopBorder;
				default:
					return style.BottomBorder;
			}
		}
		private byte GetColorShift(int index) {
			switch(index) {
				case 0:
					return 0;
				case 1:
					return 7;
				case 2:
					return 16;
				default:
					return 23;
			}
		}
		private byte GetBrushStyle(BrushStyle bs) {
			switch(bs) {
				case BrushStyle.Clear:
					return 0;
				default:
					return 1;
			}
		}
		private int GetPackedFillStyle(byte style, int fgColor, int bkColor) {
			int result = 0;
			fgColor = xlsCheckColor(fgColor, ColorItemType.BrushBKColor);
			bkColor = 0x41;
			ushort loWord = 0;
			ushort hiWord = 0;
			if(fgColor != 0x40) 
				loWord = (ushort)(style << 10);
			hiWord = (ushort)(((bkColor & 0x7F) << 7) | (fgColor & 0x7F));
			result = hiWord;
			result <<= 16;
			result += loWord;
			return result;
		}
		private bool IsIntegerValue(object data) {
			return 
				data is System.Int16 || 
				data is System.UInt16 ||
				data is System.Int32 || 
				data is System.UInt32 ||
				data is System.Int64 ||				
				data is System.UInt64 ||
				data is System.SByte ||
				data is System.Byte;				
		}
		private DynamicByteBuffer WordArrayToDynamicByteBuffer(ushort[] data) {
			DynamicByteBuffer result = new DynamicByteBuffer();
			result.Alloc(data.Length * 2);
			for(int i = 0; i < data.Length; i++)
				result.SetElements(i * 2, BitConverter.GetBytes(data[i]), 2);
			return result;
		}
		protected string GetCloneFileName(string fileName) {
			string result = fileName;
			if(result == "")
				result = this.FileName;
			return result;
		}
		protected Stream GetCloneStream(Stream stream) {
			return (stream != null)? stream: this.Stream;
		}
		protected virtual int CalculateStoredSize() {
			int result = XlsConsts.DefaultDataSize;
			result += styles.GetFullSize();
			result += fonts.GetFullSize();
			result += this.unicodeSheetNameBytes.Length;
			result += sst.PackedSize;
			result += colStyles.GetFullSize();
			result += rowStyles.GetFullSize();
			result += CalculatePicturesStoredSize();
			result += cells.FullSize;
			if(unionCellsCount > 0) {
				int size = unionCellsCount << 3;
				result += size + (int)Math.Ceiling(((double)size / 0x2000)) * 6;
			}
			foreach(XlsHyperlink hyperlink in hyperlinks)
				result += hyperlink.GetSize();
			return result;
		}
		protected int CalculatePicturesStoredSize() {
			if(!picturesWriter.IsReady) 
				return 0;
			int result = 0;
			Stream innerStream = this.stream.InnerStream;
			long pos = innerStream.Position;
			CalculationXlsStream calcStream = new CalculationXlsStream(innerStream);
			try {
				picturesWriter.WriteMsoDrawingGroup(calcStream);
				picturesWriter.WriteMsoDrawing(calcStream);
				result = calcStream.CalculatedSize;
			} finally {
				innerStream.Seek(pos, SeekOrigin.Begin);
				calcStream.Close();
			}
			return result;
		}
		protected ushort xlsCheckColor(int color, ColorItemType itemType) {
			ushort result = 0;
			switch(itemType) {
				case ColorItemType.FontColor:
					if(color == 0)
						result = 0x7FFF;
					break;
				case ColorItemType.BrushBKColor:
					if(color == ColorTranslator.ToWin32(SystemColors.Window))
						result = 0x40;
					break;
				case ColorItemType.BrushFGColor:
				case ColorItemType.BorderColor:
					if(color == 0)
						result = 0x40;
					break;
			}
			if(result > 0)
				return result;
			for(int i = 55; i >= 0; i--) {
				if(palette[i] == color) {
					if(i <= usedColors) {
						if(i != usedColors) {
							int c = palette[usedColors];
							palette[usedColors] = palette[i];
							palette[i] = c;
						}
						result = (ushort)(usedColors + 8);
						usedColors--;
					} else
						result = (ushort)(i + 8);
					return result;
				}
			}
			if(usedColors >= 0) {
				palette[usedColors] = color;
				result = (ushort)(usedColors + 8);
				usedColors--;
			} else {
				switch(itemType) {
					case ColorItemType.FontColor:
						result = 0x7FFF;
						break;
					default:
						result = 0x40;
						break;
				}
			}
			return result;
		}
		protected bool xlsCheckPos(int col, int row) {
			if(maxCol < 0 || maxRow < 0)
				throw new ExportCacheException(invalidCellDimension);
			return (col < maxCol) && (row < maxRow) && (col >= 0) && (row >= 0);
		}
		protected void xlsCreateStyles() {
			if(cells.CellsList != null) {
				for(int i = 0; i < cells.CellsList.Length; i++) {
					if(cells.CellsList[i].XF >= XlsConsts.CountOfXFStyles &&
						(cells.CellsList[i].RecType & XlsConsts.MergeState) != XlsConsts.MergeState) {
						int styleIndex = cells.CellsList[i].XF - XlsConsts.CountOfXFStyles;
						ushort initialRecType = cells.CellsList[i].RecType;
						cells.CellsList[i].XF = (ushort)xlsRegisterStyle(styleCache[styleIndex], ref cells.CellsList[i].RecType);
						styleCache.MarkStyleAsExported(styleIndex, cells.CellsList[i].XF, initialRecType);
					}
					OnProviderProgress(i * 80 / cells.CellsList.Length);
				}
			}
			OnProviderProgress(80);
		}
		protected int xlsRegisterFont(ExportCacheCellStyle style) {
			int size = (style.TextFont.Name.Length << 1) + 16;
			string name = style.TextFont.Name;
			DynamicByteBuffer font  = new DynamicByteBuffer();
			font.Alloc(size + 6);
			font.SetElements(0, BitConverter.GetBytes((ushort)size));
			font.SetElements(2, BitConverter.GetBytes((ushort)((int)style.TextFont.Size * 20)));
			ushort a = 0;
			ushort b = 0;
			if(style.TextFont.Italic)
				a = 0x02;
			if(style.TextFont.Strikeout)
				b = 0x08;
			font.SetElements(4, BitConverter.GetBytes(a | b));
			font.SetElements(6, BitConverter.GetBytes(
				xlsCheckColor(ColorTranslator.ToWin32(style.TextColor), ColorItemType.FontColor)));
			a = (ushort)(style.TextFont.Bold ? 0x2BC: 0x190);
			font.SetElements(8, BitConverter.GetBytes(a));
			font.SetElement(12, Convert.ToByte(style.TextFont.Underline));
			font.SetElement(14, style.TextFont.GdiCharSet);
			font.SetElement(16, (byte)name.Length);
			font.SetElement(17, (byte)1);
			font.SetElements(18, StringToByteArray(name));
			font.SetElements(size + 2, BitConverter.GetBytes((int)0));
			return fonts.AddUniqueData(font) + 6;
		}
		protected int xlsRegisterStyle(ExportCacheCellStyle style, ref ushort type) {
			ushort initialType = type;
			ushort preparedCellType = XlsCellData.PrepareCellStyle(ref type);
			if(style.WasExportedWithType(initialType)) {
				return style.GetExportResult(initialType);
			}
			DynamicByteBuffer XF = new DynamicByteBuffer();
			XF.Alloc(26);
			XF.SetElements(0, XlsConsts.XF_, 15 * 24 + 2, 22);
			XF.SetElements(2, BitConverter.GetBytes(
				(ushort)(xlsRegisterFont(style)) & 0xFFFF));
			if(!String.IsNullOrEmpty(style.FormatString))
				preparedCellType = FormatStringConverter.CreateInstance(preparedCellType, style.FormatString).GetCellType(preparedCellType);
			XF.SetElements(4, BitConverter.GetBytes(preparedCellType));
			ushort xfStyleState = 0x0400 | 0x0800 | 0x1000 | 0x2000 | 0x4000 | 0x8000;
			ushort temp = (ushort)(BitConverter.ToUInt16(XF.GetElements(10, 2), 0) | xfStyleState);
			XF.SetElements(10, BitConverter.GetBytes(temp));
			byte alignment = 1;
			if(style.TextAlignment == StringAlignment.Center)
				alignment = 2;
			else if(style.TextAlignment == StringAlignment.Far)
				alignment = 3;
			if(style.LineAlignment == StringAlignment.Center)
				alignment += 16;
			else if(style.LineAlignment == StringAlignment.Far)
				alignment += 32;
			XF.SetElements(8, BitConverter.GetBytes((ushort)(alignment | 8)));
			temp = BitConverter.ToUInt16(XF.GetElements(10, 2), 0);
			temp |= (ushort)(Convert.ToByte(false) << 5);
			XF.SetElements(10, BitConverter.GetBytes(temp));
			byte[] leftRightBorders = new byte[4] {0, 2, 1, 3};
			byte W = 0;
			for(int i = 0; i < 4; i++) {
				ExportCacheCellBorderStyle border = GetBorderStyle(style, i);
				if(!border.IsDefault && border.Width > 0) {
					if(border.Width == 2)
						W = 2;
					else {
						if(border.Width > 2)
							W = 5;
						else
							W = 1;
					}   
					temp = BitConverter.ToUInt16(XF.GetElements(12, 2), 0);
					temp |= (ushort)(W << (4 * i));
					XF.SetElements(12, BitConverter.GetBytes(temp));
					int temp2 = BitConverter.ToInt32(XF.GetElements(14, 4), 0);
					temp2 |= (xlsCheckColor(ColorTranslator.ToWin32(border.Color_),
						ColorItemType.BorderColor) << GetColorShift(i));
					XF.SetElements(14, BitConverter.GetBytes(temp2));
				}
			}
			if(GetBrushStyle(style.BrushStyle_) != 0)
				XF.SetElements(18, BitConverter.GetBytes(
					GetPackedFillStyle(GetBrushStyle(style.BrushStyle_), 
					ColorTranslator.ToWin32(style.BkColor),
					ColorTranslator.ToWin32(style.FgColor))));
			XF.SetElements(22, BitConverter.GetBytes((int)0));
			int result = styles.AddUniqueData2(XF);
			result += XlsConsts.CountOfXFStyles;
			return result;
		}
		protected void xlsWriteBuf(byte[] recData) {
			stream.Write(recData, 0, recData.Length);
		}
		protected void xlsWriteHeader() {
			xlsCreateStyles();
			picturesWriter.CreateObjectHierarchy(Pictures, cells.Pictures);
			workBoolWriter.CreateOleStream(CalculateStoredSize(), stream);
			int pos = (int)stream.Position;
			XlsConsts.BOF[6] = 0x05;
			xlsWriteBuf(XlsConsts.BOF);
			xlsWriteBuf(XlsConsts.TabID);
			xlsWriteBuf(XlsConsts.WINDOW1);
			for(int i = 0; i <= 4; i++)
				xlsWriteBuf(XlsConsts.Font_);
			fonts.SaveToStream(stream);
			xlsWriteBuf(XlsConsts.XF_);
			styles.SaveToStream(stream);
			stream.Write(BitConverter.GetBytes(XlsConsts.Palette), 0, 2);
			ushort sizeOfPalette = (ushort)(palette.Length * 4);
			stream.Write(BitConverter.GetBytes((ushort)(sizeOfPalette + 2)), 0, 2);
			stream.Write(BitConverter.GetBytes((ushort)56), 0, 2);
			for(int i = 0; i < 56; i++)
				stream.Write(BitConverter.GetBytes(palette[i]), 0, 4);
			stream.Write(XlsConsts.STYLE, 0, XlsConsts.SizeOfSTYLE);
			int sheetPos = (int)stream.Position + 4;
			stream.Write(BitConverter.GetBytes(XlsConsts.BoundSheet), 0, 2);
			stream.Write(BitConverter.GetBytes((ushort)(this.unicodeSheetNameBytes.Length + 8)), 0, 2);
			stream.Write(BitConverter.GetBytes((ushort)0), 0, 2);
			stream.Write(BitConverter.GetBytes((ushort)0), 0, 2);
			stream.Write(BitConverter.GetBytes((ushort)0), 0, 2);
			stream.Write(BitConverter.GetBytes((byte)this.unicodeSheetNameBytes.Length / 2), 0, 1);
 			stream.Write(BitConverter.GetBytes((byte)1), 0, 1);
			for(int i = 0; i < this.unicodeSheetNameBytes.Length; i++)
				stream.Write(BitConverter.GetBytes((byte)this.unicodeSheetNameBytes[i]), 0, 1);
			picturesWriter.WriteMsoDrawingGroup(stream);
			sst.SaveToStream(stream, -1);
			xlsWriteBuf(XlsConsts.SupBook);
			xlsWriteBuf(XlsConsts.ExternSheet);
			xlsWriteBuf(XlsConsts.EOF);
			stream.Seek(sheetPos, SeekOrigin.Begin);
			sheetPos = (int)stream.Length - pos;
			stream.Write(BitConverter.GetBytes(sheetPos), 0, 4);
			stream.Seek(0, SeekOrigin.End);
		}
		protected void xlsWriteWorkBook() {
			XlsConsts.BOF[6] = 0x10;
			xlsWriteBuf(XlsConsts.BOF);
			DynamicByteBuffer dimension = new DynamicByteBuffer(XlsConsts.Dimension);
			dimension.SetElements(2 * 4, BitConverter.GetBytes((int)(maxRow)));
			dimension.SetElements(7 * 2, BitConverter.GetBytes((ushort)(maxCol)));
			xlsWriteBuf(XlsConsts.Dimension);
			DynamicByteBuffer window2 = new	DynamicByteBuffer(XlsConsts.WINDOW2);
			if(visibleGrid)
				window2.SetElements(2 * 2, BitConverter.GetBytes((ushort)0x6B6));
			else
				window2.SetElements(2 * 2, BitConverter.GetBytes((ushort)0x6B4));
			picturesWriter.WriteMsoDrawing(stream);
			xlsWriteBuf(XlsConsts.WINDOW2);
			colStyles.SaveToStream(stream);
			rowStyles.SaveToStream(stream);
			cells.SaveToStream(stream);
			if(unionCellsCount > 0) {
				ushort C = (ushort)Math.Min(unionCellsCount, 1024);
				ushort size = (ushort)((C << 3) + 2);
				stream.Write(BitConverter.GetBytes(XlsConsts.MergeCells), 0, 2);
				stream.Write(BitConverter.GetBytes(size), 0, 2);
				stream.Write(BitConverter.GetBytes(C), 0, 2);
				for(int i = 1; i <= unionCellsCount; i++) {
					unionCells.GetElement(i - 1).WriteToStream(stream);
					if((i % 1024) == 0 && i != unionCellsCount) {
						C = (ushort)Math.Min(unionCellsCount - i, 1024);
						size = (ushort)((C << 3) + 2);
						stream.Write(BitConverter.GetBytes(XlsConsts.MergeCells), 0, 2);
						stream.Write(BitConverter.GetBytes(size), 0, 2);
						stream.Write(BitConverter.GetBytes(C), 0, 2);
					}
				}
			}
			foreach(XlsHyperlink hyperlink in hyperlinks)
				hyperlink.WriteToStream(stream);
			xlsWriteBuf(XlsConsts.EOF);
		}
		protected bool PlaceParsedString(int col, int row, string text) {
			return true;
		}
		void SetCellStringInternal(int col, int row, string str) {
			str = ValidateString(str);
			if(str.Length > 0) {
				if(str.Length <= XlsConsts.MaxLenShortStringW)
					cells.SetCellDataString(col, row, str);
				else
					cells.SetCellDataSSTString(col, row, sst.Add(str));
			}
		}
		#region IExportProvider implementation
		public void Commit() {
			try {
				OnProviderProgress(0);
				xlsWriteHeader();
				OnProviderProgress(90);
				xlsWriteWorkBook();
				OnProviderProgress(100);
			}
			finally {
				if(IsStreamMode)
					stream.Flush();
				else
					stream.Close();
				ImageHelper.Reset();
			}
		}
		public int RegisterStyle(ExportCacheCellStyle style) {
			return styleCache.RegisterStyle(style);
		}
		public void SetDefaultStyle(ExportCacheCellStyle style) {
			styleCache.DefaultStyle = style;
		}
		public void SetStyle(ExportCacheCellStyle style) {
			for(int i = 0; i <= maxCol; i++)
				for(int j = 0; j < maxRow; j++)
					SetCellStyle(i, j, style);
		}
		public void SetStyle(int styleIndex) {
			for(int i = 0; i <= maxCol; i++)
				for(int j = 0; j < maxRow; j++)
					SetCellStyle(i, j, styleIndex);
		}
		public void SetCellStyle(int col, int row, int styleIndex) {
			if(xlsCheckPos(col, row))
				cells.GetCell(col, row).XF = (ushort)(styleIndex + XlsConsts.CountOfXFStyles);
		}
		public void SetCellStyle(int col, int row, ExportCacheCellStyle style) {
			if(xlsCheckPos(col, row))
				SetCellStyle(col, row, RegisterStyle(style));
			if(!string.IsNullOrEmpty(style.Url))
				SetHyperLink((short)col, (short)row, style.Url);
		}
		void SetHyperLink(short col, short row, string url) {
			hyperlinks.Add(new XlsHyperlink(col, row, url));
		}
		public void SetCellStyle(int col, int row, int exampleCol, int exampleRow) {
			if(xlsCheckPos(exampleCol, exampleRow))
				SetCellStyle(col, row, GetCellStyle(exampleCol, exampleRow));
		}
		public void SetCellUnion(int col, int row, int width, int height) {
			if(!xlsCheckPos(col, row))
				return;
			width = Math.Min(width, maxCol - col);
			height = Math.Min(height, maxRow - row);
			if(width == 1 && height == 1) return;
			if(unionCellsCount == unionCellsCapacity) {
				unionCellsCapacity = ((unionCellsCapacity >> 1) + 1) << 2;
				unionCells.Realloc(unionCellsCapacity);
			}
			MergeRect rect = unionCells.GetElement(unionCellsCount);
			rect.Top = (ushort)row;
			rect.Bottom = (ushort)(row + height - 1);
			rect.Left = (ushort)col;
			rect.Right = (ushort)(col + width - 1);
			unionCells.SetElement(unionCellsCount, (MergeRect)rect);
			unionCellsCount++;
			for(int i = col; i < col + width; i++)
				for(int j = row; j < row + height; j++)
					if(i != col || j != row)
						SetCellStyle(i, j, col, row);
			SetPictureUnion(col, row, width, height);
		}
		void SetPictureUnion(int col, int row, int width, int height) {
			foreach(SheetPicture pic in cells.Pictures) {
				if(pic == null)
					continue;
				if(pic.Col1 == col && pic.Row1 == row) {
					pic.Col2 = (ushort)(pic.Col1 + width - 1);
					pic.Row2 = (ushort)(pic.Row1 + height - 1);
				}
			}
		}
		public void SetCellStyleAndUnion(int col, int row, int width, int height, int styleIndex) {
			SetCellStyle(col, row, styleIndex);
			SetCellUnion(col, row, width, height);
		}
		public void SetCellStyleAndUnion(int col, int row, int width, int height, ExportCacheCellStyle style) {
			SetCellStyleAndUnion(col, row, width, height, RegisterStyle(style));
		}
		public void SetRange(int width, int height, bool isVisible) {
			maxCol = Math.Min(width, XlsConsts.MaxColumn + 1);
			maxRow = Math.Min(height, XlsConsts.MaxRow + 1);
			colStyles.Capacity = maxCol;
			rowStyles.Capacity = maxRow;
			visibleGrid = isVisible;
			cells.SetRange(maxCol, maxRow);
			ExportCacheCellStyle defaultStyle = styleCache.DefaultStyle;
			int defaultBorderWidth = 0;
			if(isVisible)
				defaultBorderWidth = 1;
			defaultStyle.LeftBorder.Width = defaultBorderWidth;
			defaultStyle.TopBorder.Width = defaultBorderWidth;
			defaultStyle.RightBorder.Width = defaultBorderWidth;
			defaultStyle.BottomBorder.Width = defaultBorderWidth;
			SetDefaultStyle(defaultStyle);
		}
		public void SetColumnWidth(int col, int width) {
			if(col > XlsConsts.MaxColumn)
				return;
			int colRecSize = 12;
			ushort[] colInfo = new ushort[(colRecSize + 2 + 1) / 2];
			colInfo[0] = (ushort)colRecSize;
			colInfo[1] = (ushort)col;
			colInfo[2] = (ushort)col;
			colInfo[3] = (ushort)Math.Round(width * 36.6);
			colInfo[4] = (ushort)0x000F;
			colStyles.AddData(WordArrayToDynamicByteBuffer(colInfo), colRecSize);
		}
		public void SetRowHeight(int row, int height) {
			if(row > XlsConsts.MaxRow)
				return;
			int rowRecSize = 16;
			ushort[] rowInfo = new ushort[(rowRecSize + 2) / 2];
			rowInfo[0] = (ushort)rowRecSize;
			rowInfo[1] = (ushort)row;
			rowInfo[3] = (ushort)0x0100;
			rowInfo[4] = (ushort)Math.Round(height * 20 / 1.325);
			rowInfo[7] = (ushort)0x01C0;
			rowInfo[8] = (ushort)0x0F;
			rowStyles.AddData(WordArrayToDynamicByteBuffer(rowInfo), rowRecSize);
		}
		public void SetCellData(int col, int row, object data) {
			if(!xlsCheckPos(col, row)) return;
			if(data is System.Boolean)
				cells.SetCellDataBoolean(col, row, (bool)data);
			else if(IsIntegerValue(data))
				cells.SetCellDataInteger(col, row, Convert.ToInt64(data));
			else if(data is System.Single || data is System.Double || data is System.Decimal)
				cells.SetCellDataDouble(col, row, Convert.ToDouble(data));
			else if(data is System.String) {
				SetCellStringInternal(col, row, (string)data);
			} else if(data is Image)
				SetCellImage(col, row, (Image)data, 0, 0);
			else if(data is System.DateTime)
				cells.SetCellDataDateTime(col, row, Convert.ToDateTime(data));
			else if(data is System.TimeSpan)
				cells.SetCellDataTimeSpan(col, row, (System.TimeSpan)data);
			else
				SetCellData(col, row, Convert.ToString(data));
		}
		public void SetCellImage(int col, int row, Image image, float relativeHorizOffset, float relativeVertOffset) {
			SheetPicture pic = SheetPicture.CreateInstance(col, row, Pictures.GetByImage(image), relativeHorizOffset, relativeVertOffset);
			cells.SetCellDataImage(pic);
		}
		public void SetCellString(int col, int row, string str) {
			if(xlsCheckPos(col, row))
				SetCellStringInternal(col, row, str);
		}
		public ExportCacheCellStyle GetStyle(int styleIndex) {
			return styleCache[styleIndex];
		}
		public ExportCacheCellStyle GetCellStyle(int col, int row) {
			if(xlsCheckPos(col, row)) {
				ushort XF = cells.GetCell(col, row).XF;
				if(XF > XlsConsts.CountOfXFStyles)
					return styleCache[XF - XlsConsts.CountOfXFStyles];
				else
					return styleCache[0];
			} else
				return new ExportCacheCellStyle();
		}
		public ExportCacheCellStyle GetDefaultStyle() {
			return styleCache.DefaultStyle;
		}
		public int GetColumnWidth(int col) {
			return 0;
		}
		public int GetRowHeight(int row) {
			return 0;
		}
		public virtual IExportProvider Clone(string fileName, Stream stream) {
			return (IsStreamMode)? new ExportXlsProvider(GetCloneStream(stream)) : new ExportXlsProvider(GetCloneFileName(fileName));
		}
		#endregion
	}	
	#region XlsStream 
	
	public class XlsStream {
		const int MaxBytesWrite = XlsConsts.MaxRecSize97;
		Stream stream;
		bool continueWriting = false;
		long writeContinuePos = 0;
		int writtenBytes = 0;
		public XlsStream(Stream stream) {
			this.stream = stream;
		}
		internal Stream InnerStream { get { return stream; } }
		public long Position { get { return stream.Position; } }
		public long Length { get { return stream.Length; } }
		public virtual void Flush() {
			stream.Flush();
		}
		public virtual void Close() {
			stream.Close();
		}
		public void BeginContinueWrite() {
			continueWriting = true;
			this.writtenBytes = 0;
			this.writeContinuePos = Position;
		}
		public long Seek (long offset, SeekOrigin origin) {
			return stream.Seek(offset, origin);
		}
		public void EndContinueWrite() {
			if(writtenBytes > 0) {
				Seek(writeContinuePos + 2, SeekOrigin.Begin);
				System.Diagnostics.Debug.Assert(writtenBytes <= Math.Min(MaxBytesWrite, (int)ushort.MaxValue));
				ushort count = (ushort)writtenBytes;
				stream.Write(BitConverter.GetBytes(count), 0, 2);
				Seek(0, SeekOrigin.End);
			} 
			continueWriting = false;
		}
		byte[] buffer = new byte[16];
		public void Write(ushort value) {
			buffer[0] = (byte)value;
			buffer[1] = (byte)(value >> 8);
			Write(buffer, 0, 2);
		}
		public void Write(byte value) {
			buffer[0] = value;
			Write(buffer, 0, 1);
		}
		public void Write(bool value) {
			buffer[0] = (byte)(value ? 1 : 0);
			Write(buffer, 0, 1);
		}
		public void Write(uint value) {
			Write((int)value);
		}
		public void Write(int value) {
			buffer[0] = (byte)value;
			buffer[1] = (byte)(value >> 8);
			buffer[2] = (byte)(value >> 16);
			buffer[3] = (byte)(value >> 24);
			Write(buffer, 0, 4);
		}
		public void Write(byte[] buffer, int offset, int count) {
			if(continueWriting)
				CheckContinueWrite(buffer, count);
			else {
				IncWrittenCount(count);
				WriteCore(buffer, offset, count);
			}
		}
		protected void CheckContinueWrite(byte[] buffer, int count) {
			if(count + writtenBytes > MaxBytesWrite) {
				int bytes = MaxBytesWrite - writtenBytes;
				Seek(writeContinuePos + 2, SeekOrigin.Begin);
				WriteCore(BitConverter.GetBytes(MaxBytesWrite), 0, 2);
				Seek(0, SeekOrigin.End);
				WriteCore(buffer, 0, bytes);
				this.writeContinuePos = Position;
				writtenBytes = 0;
				count -= bytes;
				byte[] newBuffer = new byte[count];
				Array.Copy(buffer, bytes, newBuffer, 0, count);
				WriteHeader(XlsConsts.BIFFRecId_Continue, 0);
				CheckContinueWrite(newBuffer, count);
			}
			else {
				WriteCore(buffer, 0, count);
				IncWrittenCount(count);
			}
		}
		void IncWrittenCount(int count) {
			writtenBytes += count;
		}
		public void WriteHeader(ushort recId, int len) {
			BIFFHeader header = new BIFFHeader();
			header.RecId = recId;
			header.Length = Convert.ToUInt16(len & 0xFFFF);
			header.WriteToStream(this);
		}
		internal void WriteCore(ushort value) {
			buffer[0] = (byte)value;
			buffer[1] = (byte)(value >> 8);
			WriteCore(buffer, 0, 2);
		}
		protected internal virtual void WriteCore(byte[] buffer, int offset, int count) {
			stream.Write(buffer, offset, count);
		}
	}
	#endregion
	
	public class CalculationXlsStream : XlsStream {
		int calculatedSize = 0;
		public CalculationXlsStream(Stream stream) : base(stream) {
		}
		public int CalculatedSize { get  { return calculatedSize; }  }
		protected internal override void WriteCore(byte[] buffer, int offset, int count) {
			calculatedSize += count;
		}
		public override void Flush() {
		}
		public override void Close() {
		}
	}
	
	public struct MergeRect {
		public ushort Top, Bottom, Left, Right;
		public const int SizeOf = 4 * 2;
		public void WriteToStream(XlsStream stream) {
			stream.Write(Top);
			stream.Write(Bottom);
			stream.Write(Left);
			stream.Write(Right);
		}
	}
	public enum ColorItemType {
		FontColor,
		BrushBKColor,
		BrushFGColor,
		BorderColor
	}
	#region Records
	
	public class XlsRecordItem {
		private int size;
		private DynamicByteBuffer data;
		public XlsRecordItem(int size, DynamicByteBuffer data) {
			this.size = size;
			this.data = data;
		}
		public int Size {
			get {
				return size;
			}
		}
		public DynamicByteBuffer Data {
			get {
				return data;
			}
		}
	}
	
	public class XlsRecordList: ArrayList {
		private ushort id;
		public XlsRecordList(ushort recordId): base() {
			id = recordId;
		}
		public int AddData(DynamicByteBuffer data, int size) {
			return Add(new XlsRecordItem(size, data));
		}
		public int AddUniqueData2(DynamicByteBuffer data) {
			ushort size = BitConverter.ToUInt16(data.GetElements(0, 2), 0);
			for(int i = 0; i < Count; i++) {
				DynamicByteBuffer item = this[i].Data;
				if(data.Compare(0, item.Data)) {
					data = null;
					return i;
				}
			}			
			return Add(new XlsRecordItem(size, data));
		}
		public int AddUniqueData(DynamicByteBuffer data) {
			ushort size = BitConverter.ToUInt16(data.GetElements(0, 2), 0);
			int hashCode = BitConverter.ToInt32(data.GetElements(size + 2, 4), 0);
			for(int i = 0; i < Count; i++) {
				DynamicByteBuffer item = this[i].Data;
				int offset = BitConverter.ToUInt16(item.GetElements(0, 2), 0);
				if((hashCode == BitConverter.ToInt32(item.GetElements(offset + 2, 4), 0) &&
					data.Compare(0, item.Data, 0, size))) {
					data = null;
					return i;
				}
			}
			return Add(new XlsRecordItem(size, data));
		}
		public void SaveToStream(XlsStream stream) {
			for(int i = 0; i < Count; i++) {
				stream.Write(BitConverter.GetBytes(id), 0, 2);
				DynamicByteBuffer item = this[i].Data;
				item.WriteToStream(stream, this[i].Size + 2);
			}
		}
		public int GetSize(int index) {
			return this[index].Size;
		}
		public int GetFullSize() {
			int result = Count << 2;
			for(int i = 0; i < Count; i++)
				result += this[i].Size;
			return result;
		}
		public new XlsRecordItem this[int index] {
			get {
				return (XlsRecordItem)base[index];
			}
		}
	}
	#endregion
	#region Dynamic buffers
	public class DynamicBuferException: ApplicationException {
		public DynamicBuferException(string message): base(message) {
		}
	}
	
	abstract public class DynamicBufferBase<T> {
		private List<T[]> buffer;
		protected int bufferSize;
		public DynamicBufferBase() {
			buffer = new List<T[]>();
		}
		public DynamicBufferBase(int capacity) {
			if(capacity > 0)
				buffer = new List<T[]>(capacity);
			else
				buffer = new List<T[]>();
		}
		public DynamicBufferBase(T[] data) {
			buffer = new List<T[]>();
			buffer.Add(data);
			bufferSize = data.Length;
		}
		protected void OffsetToInternalIndices(int offset, out int listIndex, out int arrayIndex) {
			int size = 0;
			listIndex = 0;
			arrayIndex = 0;
			for(int i = 0; i < Count; i++) {
				size += this[i].Length;
				if(offset < size) {
					listIndex = i;
					arrayIndex = offset - (size - this[i].Length);
					break;
				}
			}
		}
		private void CheckOutOfBuffer(bool condition) {
			if(!condition)
				throw new DynamicBuferException("offset is out of buffer");
		}
		protected T[] NewArray(int size) {
			return new T[size];
		}
		protected T GetItem(T[] array, int index) {
			return array[index];
		}
		protected void SetItem(T[] array, int index, T data) {
			array[index] = data;
		}
		protected bool EqualItems(T item1, T item2) {
			return item1.Equals(item2);
		}
		public void Realloc(int size) {
			if(size == 0) 
				Clear();
			else {
				if(size > bufferSize)
					buffer.Add(NewArray(size - bufferSize));
				bufferSize = size;
			}
		}
		public void Alloc(int size) {
			if(size > 0) {
				buffer.Add(NewArray(size));
				bufferSize += size;
			}
		}
		public void Clear() {
			buffer.Clear();
			bufferSize = 0;
		}
		public T GetElement(int offset) {
			CheckOutOfBuffer(offset >= 0 && offset < bufferSize);
			int listIndex = 0;
			int arrayIndex = 0;
			OffsetToInternalIndices(offset, out listIndex, out arrayIndex);
			return GetItem(this[listIndex], arrayIndex);
		}
		public T[] GetElements(int offset, int count) {
			if(count <= 0)
				return null;
			CheckOutOfBuffer(offset >= 0 && (offset + count) <= bufferSize);
			T[] result = NewArray(count);
			int listIndex = 0;
			int arrayIndex = 0;
			int dataOffset = 0;
			OffsetToInternalIndices(offset, out listIndex, out arrayIndex);
			for(int i = arrayIndex; i < this[listIndex].Length; i++) {
				if(dataOffset >= count)
					return result;
				SetItem(result, dataOffset, GetItem(this[listIndex], i));
				dataOffset++;
			}
			if(listIndex < (Count - 1)) {	
				for(int i = listIndex + 1; i < Count; i++) {
					for(int j = 0; j < this[i].Length; j++) {
						if(dataOffset >= count)
							return result;
						SetItem(result, dataOffset, GetItem(this[i], j));
						dataOffset++;
					}
				}
			}
			return result;
		}
		public void SetElement(int offset, T data) {
			CheckOutOfBuffer(offset >= 0 && offset < bufferSize);
			int listIndex = 0;
			int arrayIndex = 0;
			OffsetToInternalIndices(offset, out listIndex, out arrayIndex);
			SetItem(this[listIndex], arrayIndex, data);
		}
		public void SetElements(int offset, T[] data, int dOffset, int size) {
			if((dOffset + size) > data.Length || (offset + size) > bufferSize)
				return;
			CheckOutOfBuffer(dOffset >= 0 && (dOffset + size) <= data.Length);
			CheckOutOfBuffer(offset >=0 && (offset + size) <= bufferSize);
			if(data.Length > 0) {
				int listIndex = 0;
				int arrayIndex = 0;
				int dataOffset = dOffset;
				OffsetToInternalIndices(offset, out listIndex, out arrayIndex);
				for(int i = arrayIndex; i < this[listIndex].Length; i++) {
					if((dataOffset - dOffset) >= size)
						return;
					SetItem(this[listIndex], i, GetItem(data, dataOffset));
					dataOffset++;
				}
				if(listIndex < (Count - 1)) {	
					for(int i = listIndex + 1; i < Count; i++) {
						for(int j = 0; j < this[i].Length; j++) {
							if((dataOffset - dOffset) >= size)
								return;
							SetItem(this[i], j, GetItem(data, dataOffset));
							dataOffset++;
						}
					}
				}
			}
		}
		public void SetElements(int offset, T[] data, int size) {
			SetElements(offset, data, 0, size);
		}
		public void SetElements(int offset, T[] data) {
			SetElements(offset, data, 0, data.Length);
		}
		public void FillElements(int offset, int count, T data) {
			CheckOutOfBuffer(offset >= 0 && (offset + count) <= bufferSize);
			int listIndex = 0;
			int arrayIndex = 0;
			int dataOffset = 0;
			OffsetToInternalIndices(offset, out listIndex, out arrayIndex);
			for(int i = arrayIndex; i < this[listIndex].Length; i++) {
				if(dataOffset >= count)
					return;
				SetItem(this[listIndex], i, data);
				dataOffset++;
			}
			if(listIndex < (Count - 1)) {	
				for(int i = listIndex + 1; i < Count; i++) {
					for(int j = 0; j < this[i].Length; j++) {
						if(dataOffset >= count)
							return;
						SetItem(this[i], j, data);
						dataOffset++;
					}
				}
			}
		}
		public bool Compare(int offset, T[] data, int dOffset, int size) {
			if((dOffset + size) > data.Length || (offset + size) > bufferSize)
				return false;
			CheckOutOfBuffer(dOffset >= 0 && (dOffset + size) <= data.Length);
			CheckOutOfBuffer(offset >= 0 && (offset + size) <= bufferSize);
			if(data.Length > 0) {
				int listIndex = 0;
				int arrayIndex = 0;
				int dataOffset = dOffset;
				OffsetToInternalIndices(offset, out listIndex, out arrayIndex);
				for(int i = arrayIndex; i < this[listIndex].Length; i++) {
					if(dataOffset >= size)
						return true;
					if(!EqualItems(GetItem(this[listIndex], i), GetItem(data, dataOffset)))
						return false;
					dataOffset++;
				}
				if(listIndex < (Count - 1)) {	
					for(int i = listIndex + 1; i < Count; i++) {
						for(int j = 0; j < this[i].Length; j++) {
							if(dataOffset >= size)
								return true;
							if(!EqualItems(GetItem(this[i], j), GetItem(data, dataOffset)))
								return false;
							dataOffset++;
						}
					}
				}
				return true;
			}
			return false;
		}
		public bool Compare(int offset, T[] data, int size) {
			return Compare(offset, data, 0, size);
		}
		public bool Compare(int offset, T[] data) {
			return Compare(offset, data, 0, data.Length);
		}
		public T[] this[int index] {
			get {
				return buffer[index];
			}
		}
		public T[] Data {
			get {
				T[] result = NewArray(bufferSize);
				int k = 0;
				for(int i = 0; i < Count; i++) 
					for(int j = 0; j < this[i].Length; j++)
						SetItem(result, k++, GetItem(this[i], j));
				return result;
			}
		}
		public int Count {
			get {
				return buffer.Count;
			}
		}
		public int Size {
			get {
				return bufferSize;
			}
		}
	}
	
	public sealed class DynamicByteBuffer: DynamicBufferBase<byte> {
		public DynamicByteBuffer(): base() {
		}
		public DynamicByteBuffer(int capacity): base(capacity) {
		}
		public DynamicByteBuffer(byte[] data): base(data) {
		}
		public void WriteToStream(XlsStream stream, int size) {
			if(size > 0 && size <= bufferSize) {
				int listIndex = 0;
				int arrayIndex = 0;
				OffsetToInternalIndices(size - 1, out listIndex, out arrayIndex);
				byte[] array = null;
				for(int i = 0; i < listIndex; i++) {
					array = this[i];
					stream.Write(array, 0, array.Length);
				}
				array = this[listIndex];
				int itemSize = array.Length / this[listIndex].Length;
				stream.Write(array, 0, (arrayIndex + 1) * itemSize);
			}
		}
		public void WriteToStream(XlsStream stream) {
			for(int i = 0; i < Count; i++) {
				byte[] array = this[i];
				stream.Write(array, 0, array.Length);
			}
		}
	}
	
	public sealed class DynamicMergeRectBuffer : DynamicBufferBase<MergeRect> {
		public DynamicMergeRectBuffer(): base() {
		}
		public DynamicMergeRectBuffer(int capacity): base(capacity) {
		}
		public DynamicMergeRectBuffer(MergeRect[] data)
			: base(data) {
		}
	}
	#endregion
	#region OLE Header
	
	public class XlsWorkBookWriter {
		private DynamicByteBuffer buffer;
		private int bufferSize;
		private int streamSize;
		private int sectCount;
		private bool isSmallFile;
		private readonly OleFileHeader headerTemplate;
		#region OLE consts
		private const ulong oleSignature = 0xE11AB1A1E011CFD0;
		private const uint oleDifBlock = 0xFFFFFFFC;
		private const uint oleSpecBlock = 0xFFFFFFFD;
		private const uint oleEndOfChain = 0xFFFFFFFE;
		private const uint oleUnused = 0xFFFFFFFF;
		private const uint oleEmpty = 0x00000000;
		private const uint oleDllVersion = 0x0003003E;
		private const ushort olePlatformOrder = 0xFFFE;
		private const int oleSectorsInMasterFat = 109;
		private const int oleBlockIdPerBigBlock = 128;
		private const int oleMaxBlockIdInBigBlock = 127;
		private const int oleBigBlockShift = 9;
		private const int oleSmallBlockShift = 6;
		private const int oleReservedSectorCount = 2;
		private const uint oleMiniSectorMaxSize = 0x000001000;
		private const int oleSmallBlockSize = 1 << oleSmallBlockShift;
		private const int oleBigBlockSize = 1 << oleBigBlockShift;
		private const int oleDirBlockSize = 128;
		private const int oleIndexSize = 4;
		private const string oleRoot = "Root Entry";
		private const string oleWorkBook = "Workbook";
		#endregion
		public XlsWorkBookWriter() {			
			buffer = new DynamicByteBuffer();
			#region Ole header template
			headerTemplate = new OleFileHeader();
			headerTemplate.Signature = oleSignature;
			headerTemplate.ClsId[0] = oleEmpty;
			headerTemplate.ClsId[1] = oleEmpty;
			headerTemplate.OleVersion = oleDllVersion;
			headerTemplate.ByteOrder = olePlatformOrder;
			headerTemplate.SectorShift = oleBigBlockShift;
			headerTemplate.MiniSectorShift = oleSmallBlockShift;
			headerTemplate.Reserved = (ushort)oleEmpty;
			headerTemplate.Reserved1 = oleEmpty;
			headerTemplate.Reserved2 = oleEmpty;
			headerTemplate.CountSectFat = 1;
			headerTemplate.SectDirStart = 1;
			headerTemplate.TransSignature = oleEmpty;
			headerTemplate.MiniSectorCutOff = oleMiniSectorMaxSize;
			headerTemplate.SectMiniFatStart = oleEndOfChain;
			headerTemplate.CountSectMiniFat = oleEmpty;
			headerTemplate.SectDifStart = oleEndOfChain;
			headerTemplate.CountSectDif = oleEmpty;
			#endregion
		}
		private void Check(bool condition) {
			if(!condition)
				throw new Exception("WorkBook exception");
		}
		private int RoundDiv(int number, int denominator) {
			int result;
			result = number / denominator;
			if(number % denominator != 0)
				result++;
			return result;
		}
		private int GetDirEntryOffset(int index) {
			return ((oleReservedSectorCount << oleBigBlockShift) + index * oleDirBlockSize);
		}
		private int GetDifSectorOffset(int sector) {
			return (GetSectDifStart() + 
				(sector * oleBlockIdPerBigBlock) + 1) * 512;
		}
		private int GetFatSectorOffset(int sector) {
			if(sector == 0)
				return 512;
			else {
				if(sector < oleSectorsInMasterFat)
					return (sector + 2) * 512;
				else {
					sector -= oleSectorsInMasterFat;
					int difBlock = 0;
					while((sector - oleMaxBlockIdInBigBlock) >= 0) {
						sector -= oleMaxBlockIdInBigBlock;
						difBlock++;
					}
					return 
						(BitConverter.ToInt32(
						buffer.GetElements(
						GetDifSectorOffset(difBlock) + sector * 4, 4), 0) + 1) * 512;
				}
			}
		}
		private int GetCountSectFat() {
			return BitConverter.ToInt32(buffer.GetElements(44, 4), 0);
		}
		private int GetCountSectDif() {
			return BitConverter.ToInt32(buffer.GetElements(72, 4), 0);
		}
		private int GetSectDifStart() {
			return BitConverter.ToInt32(buffer.GetElements(68, 4), 0);
		}
		private int GetDirStartSector(int offset) {
			return BitConverter.ToInt32(buffer.GetElements(offset + 116, 4), 0);
		}
		private void IncCurrentIndexAndSetValue(uint value_, 
			ref int index, ref int curSector, ref int sector) {
			buffer.SetElements(curSector + index * 4, BitConverter.GetBytes(value_));
			if(index == oleMaxBlockIdInBigBlock) {
				sector++;
				curSector = GetFatSectorOffset(sector);
				index = 0;
			} else
				index++;
		}
		private void CreateEntry(string name, OleDirEntryType type, 
			OleDirEntry entry, int offset) {
			entry.EntryType = type;
			entry.BFlag = Convert.ToByte(type == OleDirEntryType.Stream);
			entry.LeftSib = oleUnused;
			entry.RightSib = oleUnused;
			entry.ChildSib = oleUnused;
			if(type == OleDirEntryType.Stream || type == OleDirEntryType.Root) {
				entry.NameLen = Convert.ToUInt16((name.Length + 1) << 1);
				if(entry.NameLen != 1)
					for(int i = 0; i < (entry.NameLen - 2) >> 1; i++)
						entry.Name[i] = name[i];
			}
			entry.CopyToBuffer(offset, buffer);
		}
		private void ReallocBuffer(int size) {
			bufferSize = (int)((RoundDiv(size, (int)oleMiniSectorMaxSize) + 1) * oleMiniSectorMaxSize);
			try {
				buffer.Realloc(bufferSize);
			} 
			finally {
				bufferSize = size;
			}
		}
		private void CreateHeader() {
			int offset = headerTemplate.CopyToBuffer(0, buffer);
			buffer.FillElements(offset, 109 * 4, (byte)0xFF); 
			if(!isSmallFile) {
				int countSectFat = RoundDiv(sectCount + 3, oleMaxBlockIdInBigBlock);
				buffer.SetElements(44, 
					BitConverter.GetBytes(countSectFat)); 
				int countSectDif = GetCountSectDif(); 
				if(countSectFat > oleSectorsInMasterFat) {
					int count = countSectFat - oleSectorsInMasterFat;
					countSectDif = RoundDiv(count, oleMaxBlockIdInBigBlock);
					buffer.SetElements(72, 
						BitConverter.GetBytes(countSectDif));
					buffer.SetElements(68, 
						BitConverter.GetBytes(oleSectorsInMasterFat + oleReservedSectorCount));
				}
				ReallocBuffer((countSectFat + countSectDif + oleReservedSectorCount) << oleBigBlockShift);
			} else {
				buffer.SetElements(60, 
					BitConverter.GetBytes((uint)2)); 
				buffer.SetElements(64, 
					BitConverter.GetBytes((uint)1)); 
			}
		}
		private void CreateDif() {
			int index = 0;
			int curSect = 0;
			int curDif = GetDifSectorOffset(curSect);
			int id = 0;
			for(int i = oleSectorsInMasterFat - 1; i < GetCountSectFat() - 1; i++)  {
				int sectorId = i - 108;
				if(index == oleMaxBlockIdInBigBlock) {
					buffer.SetElements(curDif + oleMaxBlockIdInBigBlock * 4, 
						BitConverter.GetBytes((uint)(sectorId + 111 + id)));
					curDif = GetDifSectorOffset(curSect + 1);
					index = 0;
					curSect++;
				}
				if(((sectorId + id - 1) % oleBlockIdPerBigBlock) == 0)
					id++;
				buffer.SetElements(curDif + index * 4, 
					BitConverter.GetBytes((uint)(sectorId + 110 + id)));
				index++;
			}
			buffer.FillElements(curDif + index * 4, (oleBlockIdPerBigBlock - index) * oleIndexSize, (byte)0xFF);
		}
		private void CreateDir() {
			int dirOffset_0 = GetDirEntryOffset(0);
			int dirOffset_1 = GetDirEntryOffset(1);
			buffer.FillElements(dirOffset_0, oleBigBlockSize, Convert.ToByte(oleEmpty));
			CreateEntry(oleRoot, OleDirEntryType.Root, new OleDirEntry(), 
				dirOffset_0);
			CreateEntry(oleWorkBook, OleDirEntryType.Stream, new OleDirEntry(),	
				dirOffset_1);
			buffer.SetElements(dirOffset_0 + 76, 
				BitConverter.GetBytes((uint)1));
			if(!isSmallFile) {
				buffer.SetElements(dirOffset_0 + 116, 
					BitConverter.GetBytes(oleEndOfChain));
				int countSectFat = GetCountSectFat();
				int countSectDif = GetCountSectDif();
				buffer.SetElements(dirOffset_1 + 116, 
					BitConverter.GetBytes(countSectFat + countSectDif + 1));
			} else {
				buffer.SetElements(dirOffset_0 + 116, 
					BitConverter.GetBytes((int)3));
				buffer.SetElements(dirOffset_0 + 120, 
					BitConverter.GetBytes(sectCount << oleBigBlockShift));
			}
			buffer.SetElements(dirOffset_1 + 120, 
				BitConverter.GetBytes(streamSize));
		}
		private void CreateFat() {
			if(!isSmallFile) {
				int countSectFat = GetCountSectFat();
				for(int i = 0; i < Math.Min(countSectFat, oleSectorsInMasterFat); i++) {
					if(i == 0)	
						buffer.SetElements(76, 
							BitConverter.GetBytes((uint)0));
					else
						buffer.SetElements(76 + (i * 4), 
							BitConverter.GetBytes(i + 1));
				}
				if(GetCountSectDif() > 0)
					CreateDif();
				CreateLocalFat();
			} else {
				buffer.SetElements(76, 
					BitConverter.GetBytes((uint)0)); 
				CreateSmallFat();
			}
		}
		private void CreateLocalFat() {
			int index = 0;
			int sector = 0;
			int dif = 0;
			int curSector = GetFatSectorOffset(sector);
			IncCurrentIndexAndSetValue(oleSpecBlock, 
				ref index, ref curSector, ref sector);
			IncCurrentIndexAndSetValue(oleEndOfChain, 
				ref index, ref curSector, ref sector);
			for(int i = 1; i < GetCountSectFat() + GetCountSectDif(); i++) {
				if(GetCountSectDif() > 0) {
					if((dif + GetSectDifStart() - 1) == i) {
						dif += oleBlockIdPerBigBlock;
						IncCurrentIndexAndSetValue(oleDifBlock, 
							ref index, ref curSector, ref sector);
						continue;
					}
				}
				IncCurrentIndexAndSetValue(oleSpecBlock, 
					ref index, ref curSector, ref sector);
			}
			int dirOffset = GetDirEntryOffset(1);
			for(int i = GetDirStartSector(dirOffset) + 1; i < GetDirStartSector(dirOffset) + sectCount; i++)
				IncCurrentIndexAndSetValue((uint)i, ref index, ref curSector, ref sector);
			IncCurrentIndexAndSetValue(oleEndOfChain, 
				ref index, ref curSector, ref sector);
			if(index != 0) {
				int i = oleBlockIdPerBigBlock - index;
				if(i > 0)
					buffer.FillElements(curSector + index * 4, i * 4, (byte)0xFF);
			}
		}
		private void CreateSmallFat() {
			int bigFatOffset = oleBigBlockSize;
			int smallFatOffset = 3 << oleBigBlockShift;
			int blockCount = RoundDiv(streamSize, oleSmallBlockSize);
			buffer.FillElements(bigFatOffset, oleBigBlockSize, (byte)0xFF); 
			buffer.SetElements(bigFatOffset, BitConverter.GetBytes(oleSpecBlock));
			buffer.SetElements(bigFatOffset + 4, BitConverter.GetBytes(oleEndOfChain));
			buffer.SetElements(bigFatOffset + 8, BitConverter.GetBytes(oleEndOfChain));
			int i = 3;
			while((i - 3) < (sectCount - 1)) {
				buffer.SetElements(bigFatOffset + (i * 4) , BitConverter.GetBytes((uint)(i + 1)));
				i++;
			}
			buffer.SetElements(bigFatOffset + (i * 4) , BitConverter.GetBytes(oleEndOfChain));
			for(i = 0; i < blockCount - 1; i++)
				buffer.SetElements(smallFatOffset + (i * 4), BitConverter.GetBytes((uint)(i + 1)));
			buffer.SetElements(smallFatOffset + ((blockCount - 1) * 4), BitConverter.GetBytes(oleEndOfChain));
			buffer.FillElements(smallFatOffset + blockCount * 4, 
				(oleBlockIdPerBigBlock - blockCount) * oleIndexSize, (byte)0xFF);
		}					  
		public void CreateOleStream(int dataSize, XlsStream dstStream) {
			Check(dataSize > 0 && dstStream != null);
			streamSize = dataSize;
			sectCount = RoundDiv(streamSize, oleBigBlockSize);
			int size = RoundDiv(sectCount, oleBlockIdPerBigBlock) + 3;
			isSmallFile = streamSize < oleMiniSectorMaxSize;
			if(!isSmallFile)
				ReallocBuffer(oleBigBlockSize * (size + RoundDiv(size, oleMaxBlockIdInBigBlock)));
			else
				ReallocBuffer(4 << oleBigBlockShift);
			CreateHeader();
			CreateDir();
			CreateFat();
			buffer.WriteToStream(dstStream, bufferSize);
		}
	}
	
	public class OleFileHeader {
		public ulong Signature;
		public ulong[] ClsId = new ulong[2];
		public uint OleVersion;
		public ushort ByteOrder;
		public ushort SectorShift;
		public ushort MiniSectorShift;
		public ushort Reserved;
		public uint Reserved1;
		public uint Reserved2;
		public uint CountSectFat;
		public uint SectDirStart;
		public uint TransSignature;
		public uint MiniSectorCutOff;
		public uint SectMiniFatStart;
		public uint CountSectMiniFat;
		public uint SectDifStart;
		public uint CountSectDif;
		public uint[] SectFat = new uint[109];
		public int CopyToBuffer(int offset, DynamicByteBuffer buffer) {
			int result = 0;
			buffer.SetElements(offset + result, BitConverter.GetBytes(Signature));
			result += 8;
			buffer.SetElements(offset + result, BitConverter.GetBytes(ClsId[0]));
			buffer.SetElements(offset + result, BitConverter.GetBytes(ClsId[1]));
			result += 16;
			buffer.SetElements(offset + result, BitConverter.GetBytes(OleVersion));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(ByteOrder));
			result += 2;
			buffer.SetElements(offset + result, BitConverter.GetBytes(SectorShift));
			result += 2;
			buffer.SetElements(offset + result, BitConverter.GetBytes(MiniSectorShift));
			result += 2;
			buffer.SetElements(offset + result, BitConverter.GetBytes(Reserved));
			result += 2;
			buffer.SetElements(offset + result, BitConverter.GetBytes(Reserved1));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(Reserved2));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(CountSectFat));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(SectDirStart));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(TransSignature));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(MiniSectorCutOff));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(SectMiniFatStart));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(CountSectMiniFat));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(SectDifStart));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(CountSectDif));
			result += 4;
			return result;
		}
	}
	public enum OleDirEntryType {
		Invalid,
		Storage,
		Stream,
		LockBytes,
		Property,
		Root
	}
	
	public class OleDirEntry {
		public char[] Name = new char[32];
		public ushort NameLen;
		public OleDirEntryType EntryType;
		public byte BFlag;
		public uint LeftSib;
		public uint RightSib;
		public uint ChildSib;
		public Guid Guid_;
		public int UserFlag;
		public ulong C_Time;
		public ulong M_Time;
		public int StartSector;
		public int Size;
		public int Reserved;
		public int CopyToBuffer(int offset, DynamicByteBuffer buffer) {
			int result = 0;
			for(int i = 0; i < 32; i++) {
				buffer.SetElements(offset + result, BitConverter.GetBytes(Name[i]));
				result += 2;
			}
			buffer.SetElements(offset + result, BitConverter.GetBytes(NameLen));
			result += 2;
			buffer.SetElement(offset + result, (byte)EntryType);
			result++;
			buffer.SetElement(offset + result, BFlag);
			result++;
			buffer.SetElements(offset + result, BitConverter.GetBytes(LeftSib));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(RightSib));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(ChildSib));
			result += 4;
			buffer.SetElements(offset + result, Guid_.ToByteArray());
			result += 16;
			buffer.SetElements(offset + result, BitConverter.GetBytes(UserFlag));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(C_Time));
			result += 8;
			buffer.SetElements(offset + result, BitConverter.GetBytes(M_Time));
			result += 8;
			buffer.SetElements(offset + result, BitConverter.GetBytes(StartSector));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(Size));
			result += 4;
			buffer.SetElements(offset + result, BitConverter.GetBytes(Reserved));
			result += 4;
			return result;
		}
	}
	#endregion
	#region SST
	public enum XlsExportOptimization {
		BySize,
		BySpeed
	}
	
	public class SSTList {
		private ArrayList list; 
		public SSTList() {
			list = new ArrayList();
		}
		public int Add() {
			return list.Add(new SSTBlock());
		}
		public int Count {
			get {
				return list.Count;
			}
		}
		public SSTBlock this[int index] {
			get {
				return (SSTBlock)list[index];
			}
		}
	}
	
	public class SSTStringInfoList {
		private ArrayList list;
		public SSTStringInfoList() {
			list = new ArrayList();
		}
		public void Add(int count) {
			for(int i = 0; i < count; i++)
				list.Add(new SSTStringInfo());
		}
		public int Count {
			get {
				return list.Count;
			}
		}
		public SSTStringInfo this[int index] {
			get {
				return (SSTStringInfo)list[index];
			}
		}
		public int Capacity {
			get {
				return list.Capacity;
			}
			set {
				if(value > 0 && value > list.Capacity)
					list.Capacity = value;
			}
		}
	}
	
	public class XlsStringTable {
		private ExtSST extSST = new ExtSST();
		private SSTList sst;
		private SSTStringInfoList stringsInfo;
		public XlsStringTable() {
			sst = new SSTList();
			stringsInfo = new SSTStringInfoList();
			Clear();
		}
		private bool IsOptimize() {
			return XlsConsts.Optimization == XlsExportOptimization.BySize;
		}
		private int GetHashCode(string string_, int count) {
			int result = 0;
			return result;
		}
		private bool CheckString(string string_, ushort block, ushort offset, ushort size) {
			byte[] str = StringToByteArray(string_, 0);
			if((size + offset) <= sst[block].DataSize)
				return sst[block].Data.Compare(offset, str, size); 
			else {
				bool result = true;
				int strPos = 0;
				int len = 0;
				while(size > 0) {
					len = sst[block].DataSize - offset;
					if(len < size) {
						result &= sst[block].Data.Compare(offset, str, strPos, len);
						size -= (ushort)len;
						strPos += len;
						offset = 1;
						block++;
					} else {
						result &= sst[block].Data.Compare(offset, str, strPos, size);
						break;
					}
				}
				return result;
			}
		}
		private int IndexOf(string string_) {
			if(!IsOptimize())
				return -1;
			ushort srcLen = Convert.ToUInt16(string_.Length);
			if(srcLen > 32768)
				srcLen = 32768;
			int result = -1;
			srcLen <<= 1;
			ushort hashCode = Convert.ToUInt16(GetHashCode(string_, srcLen));
			for(int i = 0; i < UniqueStringCount; i++) {
				if(stringsInfo[i].HashCode == hashCode && 
					srcLen == stringsInfo[i].StrSize) {
					if(CheckString (string_, stringsInfo[i].Block, 
						(ushort)((int)stringsInfo[i].Offset + 3),
						stringsInfo[i].StrSize)) {
						result = i;
						break;
					}
				}
			}
			return result;
		}
		private int AddBlock() {
			int result = SSTLength;
			sst.Add();
			sst[result].FillElements(0);
			sst[result].RecType = XlsConsts.Continue;
			return result;
		}
		private void AddStringInfo(string string_, ushort block, ushort offset, ushort size) {
			int infoCount = stringsInfo.Count;
			int infoIndex = UniqueStringCount;
			if(infoCount <= UniqueStringCount) {
				stringsInfo.Capacity += 512;
				stringsInfo.Add(512);
			}
			stringsInfo[infoIndex].HashCode = (ushort)GetHashCode(string_, size);
			stringsInfo[infoIndex].StrSize = size;
			stringsInfo[infoIndex].Block = block;
			stringsInfo[infoIndex].Offset = offset;
		}
		private void AddStringToBlock(byte[] str, SSTBlock dest, ushort size) {
			dest.Data.SetElement(dest.DataSize, (byte)1);
			dest.DataSize++;
			byte[] buf;
			if(str.Length > size) {
				buf = new byte[size];
				for(int i = 0; i < size; i++)
					buf[i] = str[i];
			} else
				buf = str;
			dest.Data.SetElements(dest.DataSize, buf);
			dest.DataSize += size;
		}
		private byte[] StringToByteArray(string string_, int offset) {
			int length = string_.Length * 2;
			if(offset < 0 || offset >= length)
				return null;
			byte[] result = new byte[length];
			int j = 0;
			for(int i = 0; i < string_.Length; i++) {
				result[j] = ((byte)string_[i]);
				result[j + 1] = (byte)(((short)string_[i]) >> 8);
				j += 2;
			}
			byte[] res = new byte[length - offset];
			for(int i = offset; i < length; i++)
				res[i - offset] = result[i];
			return res;
		}
		private void InsertStr(string string_) {
			short endBlock = (short)(SSTLength - 1);
			if(endBlock < 0) {
				endBlock = (short)AddBlock();
				sst[endBlock].RecType = XlsConsts.SST;
				sst[endBlock].DataSize = 8;
				sst[endBlock].StringOffset = 8;
			}
			ushort strSize = (ushort)string_.Length;
			if(strSize > 32768)
				strSize = 32768;
			strSize <<= 1;
			if((sst[endBlock].DataSize + 4) > XlsConsts.MaxBlockSize)
				endBlock = (short)AddBlock();
			ushort writeSize = (ushort)(XlsConsts.MaxBlockSize - 
				(sst[endBlock].DataSize + 3));
			if(writeSize > strSize)
				writeSize = strSize;
			else {
				if((writeSize & 0x1) != 0)
					writeSize--;
			}
			if(sst[endBlock].StringCount == 0)
				sst[endBlock].StringOffset = sst[endBlock].DataSize;
			AddStringInfo(string_, (ushort)endBlock, sst[endBlock].DataSize, strSize);
			sst[endBlock].StringCount++;
			sst[endBlock].Data.SetElements(sst[endBlock].DataSize, 
				BitConverter.GetBytes((ushort)(strSize >> 1)));
			sst[endBlock].DataSize += 2;
			AddStringToBlock(StringToByteArray(string_, 0), sst[endBlock], writeSize);
			ushort offset = 0;
			while((strSize - writeSize) > 0) {
				offset += writeSize;
				strSize -= writeSize;
				endBlock = (short)AddBlock();
				if(strSize > (XlsConsts.MaxBlockSize - 1))
					writeSize = (ushort)(XlsConsts.MaxBlockSize - 1);
				else
					writeSize = strSize;
				AddStringToBlock(StringToByteArray(string_, offset), sst[endBlock], writeSize);
			}
		}
		private int GetSkipSize(ushort block) {
			int result = 4;
			for(int i = 1; i < block; i++)
				result += sst[i].DataSize;
			return result;
		}
		private void CreateExtSST(int sstOffset) {
			if(SSTLength == 0)
				return;
			ushort stringCount = 8;
			int blocksCount = 1;
			while((UniqueStringCount - stringCount * blocksCount) > 0) {
				stringCount += 8;
				if(blocksCount < 127)
					if((UniqueStringCount - stringCount * blocksCount) > 0)
						blocksCount++;
			}
			while(((blocksCount - 1) * stringCount) > UniqueStringCount)
				blocksCount--;
			extSST.DataSize = (ushort)(2 + blocksCount * 8);
			extSST.StringPerBlock = stringCount;
			for(int i = 0; i < blocksCount; i++) {
				extSST.Data[i].StreamOffset = 
					sstOffset +
					GetSkipSize(stringsInfo[i * stringCount].Block) +
					stringsInfo[i * stringCount].Offset;
			}
		}
		public int Add(string string_) {
			if(string_.Length > 4096)
				string_ = string_.Remove(4096, string_.Length - 4096);
			int result = IndexOf(string_);
			int val = 0;
			if(result == -1) {
				result = UniqueStringCount;
				InsertStr(string_);
				val = BitConverter.ToInt32(sst[0].Data.GetElements(1 * 4, 4), 0);
				val++;
				sst[0].Data.SetElements(1 * 4, BitConverter.GetBytes(val));
			}
			val = BitConverter.ToInt32(sst[0].Data.GetElements(0, 4), 0);
			val++;
			sst[0].Data.SetElements(0, BitConverter.GetBytes(val));
			return result;
		}
		public void Clear() {
			extSST.RecType = XlsConsts.ExtSST;
		}
		public void SaveToStream(XlsStream stream, int position) {
			if(position < 0)
				position = (int)stream.Position;
			CreateExtSST(position);
			for(int i = 0; i < SSTLength; i++) 
				sst[i].WriteToStream(stream);
			if(extSST.DataSize > 0)
				extSST.WriteToStream(stream);
		}
		public int UniqueStringCount {
			get {
				if(sst.Count > 0)
					return BitConverter.ToInt32(sst[0].Data.GetElements(1 * 4, 4), 0);
				else
					return 0;
			}
		}
		public int SSTLength {
			get {
				return sst.Count;
			}
		}
		public int PackedSize {
			get {
				int result = 0;
				for(int i = 0; i < SSTLength; i++)
					result += sst[i].DataSize + 4;
				if(result != 0) {
					CreateExtSST(0);
					result += extSST.DataSize + 4;
				}
				return result;
			}
		}
	}
	
	public class SSTBlock {
		public ushort StringCount;
		public ushort StringOffset;
		public ushort RecType;
		public ushort DataSize;
		public DynamicByteBuffer Data;
		public SSTBlock() {
			Data = new DynamicByteBuffer();
			Data.Alloc(8192);
		}
		public void FillElements(byte data) {
			StringCount = (ushort)data;
			StringOffset = (ushort)data;
			RecType = (ushort)data;
			DataSize = (ushort)data;
			Data.FillElements(0, Data.Size, (byte)0);
		}
		public void WriteToStream(XlsStream stream) {
			stream.Write(BitConverter.GetBytes(RecType), 0, 2);
			stream.Write(BitConverter.GetBytes(DataSize), 0, 2);
			Data.WriteToStream(stream, DataSize);
		}
	}
	
	public class ExtSSTBlock {
		public int StreamOffset;
		public ushort StringOffset;
		public ushort Reserved;
		public void WriteToStream(XlsStream stream, ref int size) {
			if(size >= 4) {
				stream.Write(BitConverter.GetBytes(StreamOffset), 0, 4);
				size -= 4;
				if(size >= 2) {
					stream.Write(BitConverter.GetBytes(StringOffset), 0, 2);
					size -= 2;
					if(size >= 2) {
						stream.Write(BitConverter.GetBytes(Reserved), 0, 2);
						size -= 2;
					}
				}
			}
		}
	}
	
	public class ExtSST {
		public ushort RecType;
		public ushort DataSize;
		public ushort StringPerBlock;
		public ExtSSTBlock[] Data;
		public ExtSST() {
			Data = new ExtSSTBlock[256];
			for(int i = 0; i < 256; i++)
				Data[i] = new ExtSSTBlock();
		}
		public void WriteToStream(XlsStream stream) {
			stream.Write(BitConverter.GetBytes(RecType), 0, 2);
			stream.Write(BitConverter.GetBytes(DataSize), 0, 2);
			int size = DataSize;
			if(size >= 2) {
				stream.Write(BitConverter.GetBytes(StringPerBlock), 0, 2);
				size -= 2;
				if(size > 0) {
					for(int i = 0; i < 256; i++) {
						Data[i].WriteToStream(stream, ref size);
						if(size <= 0)
							break;
					}
				}
			}
		}
	}
	
	public class SSTStringInfo {
		public ushort HashCode;
		public ushort StrSize;
		public ushort Block;
		public ushort Offset;
	}
	#endregion
	#region Cells
	public enum XlsCellKind : byte {
		None,
		Integer,
		Double,
		BoolAndBool,
		String
	}
	
	public class XlsCell {
		public XlsCellKind Kind;
		public ushort RecType;
		public ushort RecSize {
			get {
				switch(Kind) {
					case XlsCellKind.BoolAndBool: return XlsConsts.BlankCellSize + 2;
					case XlsCellKind.None: return 6;
					case XlsCellKind.Double: return XlsConsts.BlankCellSize + 8;
					case XlsCellKind.Integer: return XlsConsts.BlankCellSize;
					case XlsCellKind.String: return (ushort)((Text.Length << 1) + 3 + 6);
				}
				throw new InvalidOperationException();
			}
		}
		public ushort XF;
		public int SSTIndex { get { return (int)Num; } set { Num = value; } }
		public double Num;
		public bool BoolErrValue { get { return (SSTIndex & 1) == 1; } set { SSTIndex = value ? (SSTIndex | 1) : (SSTIndex & 2); } }
		public bool ErrFlag { get { return (SSTIndex & 2) == 2; } set { SSTIndex = value ? (SSTIndex | 2) : (SSTIndex & 1); } }
		public string Text;
		public XlsCell() {
			Kind = XlsCellKind.None;
		}
		public void WriteToStream(XlsStream stream, ushort row, ushort col) {
			stream.Write(BitConverter.GetBytes(RecType), 0, 2);
			stream.Write(BitConverter.GetBytes(RecSize), 0, 2);
			stream.Write(BitConverter.GetBytes(row), 0, 2);
			stream.Write(BitConverter.GetBytes(col), 0, 2);
			stream.Write(BitConverter.GetBytes(XF), 0, 2);
			switch(Kind) {
				case XlsCellKind.Integer:
					stream.Write(SSTIndex);
					break;
				case XlsCellKind.Double:
					stream.Write(BitConverter.GetBytes(Num), 0, 8);
					stream.Write((int)0);
					break;
				case XlsCellKind.BoolAndBool:
					stream.Write(BoolErrValue);
					stream.Write(ErrFlag);
					stream.Write((int)0);
					break;
				case XlsCellKind.String:
					stream.Write((ushort)Text.Length);
					stream.Write(true);
					foreach(char ch in Text)
						stream.Write(BitConverter.GetBytes(ch), 0, 2);
					break;
			};
		}
	}
	
	public class XlsCellData {
		public static ushort PrepareCellStyle(ref ushort type) {
			ushort[] formats = { XlsConsts.DateTimeFormat, XlsConsts.DateFormat, XlsConsts.HourMinuteSecondFormat, XlsConsts.CurrencyNoneDecimalFormat };
			if((type & 0x1000) != 0) {
				ushort result = formats[type ^ 0x1000];
				type = XlsConsts.Number;
				return result == XlsConsts.DateFormat ? XlsConsts.DateTimeFormat : result;
			} else
				return 0;
		}
		private XlsCell[] cellsList;
		private int cellPerCol;
		SheetPictureCollection pictures;
		public XlsCellData() {
			pictures = new SheetPictureCollection();
		}
		public SheetPictureCollection Pictures { get { return pictures; } }
		public ushort GetDateTimeFormat(ref double value_) {
			int val = (int)value_;
			if(val != 0 && val <= 60)
				value_--;
			ushort result = XlsConsts.DateTime;
			if(val == 0)
				result = XlsConsts.Time;
			else {
				if((value_ - val) == 0)
					result = XlsConsts.Date;
			}
			return result;
		}
		public XlsCell GetCell(int col, int row) {
			return cellsList != null ? cellsList[cellPerCol * col + row] : null;
		}
		public void SetCellDataBoolean(int col, int row, bool value_) {
			XlsCell cell = GetCell(col, row);
			if(cell != null) {
				cell.RecType = XlsConsts.BoolErr;
				cell.Kind = XlsCellKind.BoolAndBool;
				cell.BoolErrValue = value_;
				cell.ErrFlag = false;
			}
		}
		public void SetCellDataBlank(int col, int row) {
			XlsCell cell = GetCell(col, row);
			if(cell != null) {
				if(cell.RecType != XlsConsts.Blank) {
					cell.RecType = XlsConsts.Blank;
					cell.Kind = XlsCellKind.None;
				}
			}
		}
		public void SetCellDataDateTime(int col, int row, DateTime value_) {
			XlsCell cell = GetCell(col, row);
			if(cell != null) {
				double v = value_.ToOADate();
				cell.RecType = GetDateTimeFormat(ref v);
				cell.Kind = XlsCellKind.Double;
				cell.Num = v;
			}
		}
		public void SetCellDataTimeSpan(int col, int row, TimeSpan value_) {
			XlsCell cell = GetCell(col, row);
			if(cell != null) {
				DateTime dateTime = new DateTime().Add(value_);
				double v = dateTime.ToOADate();
				cell.RecType = XlsConsts.Time;
				cell.Kind = XlsCellKind.Double;
				cell.Num = v;
			}
		}
		public void SetCellDataDouble(int col, int row, double value_) {
			XlsCell cell = GetCell(col, row);
			if(cell != null) {
				cell.RecType = XlsConsts.Number;
				cell.Kind = XlsCellKind.Double;
				cell.Num = value_;
			}
		}
		public void SetCellDataInteger(int col, int row, long value_) {
			SetCellDataDouble(col, row, value_);
		}
		public void SetCellDataSSTString(int col, int row, int index) {
			XlsCell cell = GetCell(col, row);
			if(cell != null) {
				cell.RecType = XlsConsts.LabelSST;
				cell.Kind = XlsCellKind.Integer;
				cell.SSTIndex = index;
			}
		}
		public void SetCellDataString(int col, int row, string text) {
			XlsCell cell = GetCell(col, row);
			if(cell != null) {
				cell.RecType = XlsConsts.Label;
				cell.Text = text;
				cell.Kind = XlsCellKind.String;
			}
		}
		public void SetCellDataImage(SheetPicture pic) {
			pictures.Add(pic);
			pic.XlsPicture.RefCount++;
		}
		public void SetRange(int colCount, int rowCount) {
			cellsList = new XlsCell[colCount * rowCount];
			cellPerCol = rowCount;
			for(int i = 0; i < colCount * rowCount; i++) {
				cellsList[i] = new XlsCell();
				cellsList[i].RecType = XlsConsts.Blank;
				cellsList[i].XF = XlsConsts.CountOfXFStyles;
			}
		}
		public void SaveToStream(XlsStream stream) {
			if(cellsList != null) {
				for(int i = 0; i < cellsList.Length; i++) {
					XlsCell cell = cellsList[i];
					ushort temp = XlsConsts.MergeState;
					temp = (ushort)~temp;
					cell.RecType &= temp;
					if(cell.RecType != 0)
						cell.WriteToStream(stream, (ushort)(i % cellPerCol), (ushort)(i / cellPerCol));
				}
			}
		}
		public int FullSize {
			get {
				int result = 0;
				if(cellsList != null) {
					for(int i = 0; i < cellsList.Length; i++) {
						XlsCell cell = cellsList[i];
						if(cell.RecType != 0) {
							result += cell.RecSize;
							result += 4;
						}
					}
				}
				return result;
			}
		}
		public XlsCell[] CellsList {
			get {
				return cellsList;
			}
		}
	}
	
	public class XlsHyperlink {
		short col;
		short row;
		string url;
		public short Column { get { return col; } }
		public short Row { get { return row; } }
		public string Url { get { return url; } }
		public XlsHyperlink(short col, short row, string url) {
			this.col = col;
			this.row = row;
			this.url = url;
		}
		public int GetSize() {
			return 26 + (XlsConsts.SizeOfHyperlinkData + Url.Length) * 2;
		}
		public void WriteToStream(XlsStream stream) {
			System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();
			short zeroWord = 0;
			byte shortSize = 2;
			byte fourByteSize = 4;
			stream.WriteHeader(XlsConsts.BIFFRecId_Hlink, GetSize() - 4);
			stream.Write(BitConverter.GetBytes(Row), 0, shortSize);
			stream.Write(BitConverter.GetBytes(Row), 0, shortSize);
			stream.Write(BitConverter.GetBytes(Column), 0, shortSize);
			stream.Write(BitConverter.GetBytes(Column), 0, shortSize);
			stream.Write(XlsConsts.HyperlinkData1, 0, XlsConsts.SizeOfHyperlinkData);
			stream.Write(BitConverter.GetBytes(0x2), 0, fourByteSize);
			stream.Write(BitConverter.GetBytes(0x3), 0, fourByteSize);
			stream.Write(XlsConsts.HyperlinkData2, 0, XlsConsts.SizeOfHyperlinkData);
			short length = (short)(Url.Length * 2);
			stream.Write(BitConverter.GetBytes(length + 2), 0, shortSize);
			stream.Write(BitConverter.GetBytes(zeroWord), 0, shortSize);
			stream.Write(encoding.GetBytes(Url), 0, length);
			stream.Write(BitConverter.GetBytes(zeroWord), 0, shortSize);
		}
	}
	
	public class XlsConsts {
		private XlsConsts() {
		}
		public static XlsExportOptimization Optimization = XlsExportOptimization.BySpeed;
		public const ushort MaxColumn	  	   = 0xFF;
		public const ushort MaxRow			   = 0xFFFF;
		public const int MaxBlockSize		   = 8192;
		public const int BlankCellSize		   = 10;
		public const int CountOfXFStyles	   = 21;
		public const ushort MaxLenShortStringA = 0xFF;
		public const ushort MaxLenShortStringW = MaxLenShortStringA >> 1;
		public const ushort Font	   = 0x0031; 
		public const ushort XF		 = 0x00E0; 
		public const ushort COLINFO	= 0x007D; 
		public const ushort Row		= 0x0208; 
		public const ushort Palette	= 0x0092; 
		public const ushort BoundSheet = 0x0085; 
		public const ushort MergeCells = 0x00E5; 
		public const ushort Currency   = 0x1003; 
		public const ushort DateTime   = 0x1000; 
		public const ushort Date	   = 0x1001; 
		public const ushort Time	   = 0x1002; 
		public const ushort MergeState = 0x2000; 
		public const ushort BoolErr	= 0x0205; 
		public const ushort Blank	  = 0x0201; 
		public const ushort Number	 = 0x0203; 
		public const ushort Label	  = 0x0204; 
		public const ushort LabelSST   = 0x00FD; 
		public const ushort ExtSST	 = 0x00FF; 
		public const ushort SST		= 0x00FC; 
		public const ushort Continue   = 0x003C; 
		#region Format Strings
		public const ushort GeneralFormat = 0x00;				
		public const ushort NoneDecimalFormat = 0x01;			
		public const ushort DecimalFormat = 0x02;				
		public const ushort DigitNoneDecimalFormat = 0x03;		
		public const ushort DigitDecimalFormat = 0x04;			
		public const ushort CurrencyNoneDecimalFormat = 0x05;	
		public const ushort CurrencyDecimalFormat = 0x07;   	
		public const ushort PercentNoneDecimalFormat = 0x09;	
		public const ushort PercentDecimalFormat = 0x0a;		
		public const ushort ExponentalDecimalFormat = 0x0b;		
		public const ushort ExponentalDecimalOneFormat = 0x30;	
		public const ushort DateFormat = 0x0e;					
		public const ushort DayMontnYearFormat = 0x0f;			
		public const ushort DayMontnFormat = 0x10;				
		public const ushort MontnYearFormat = 0x11;				
		public const ushort HourMinuteAMPMFormat = 0x12;		
		public const ushort HourMinuteSecondAMPMFormat = 0x13;	
		public const ushort HourMinuteFormat = 0x14;			
		public const ushort HourMinuteSecondFormat = 0x15;		
		public const ushort DateTimeFormat = 0x16;				
		public const ushort AccountFormat = 0x25;			   
		public const ushort AccountDecimalFormat = 0x27;		
		public const ushort MinuteSecondFormat = 0x2d;			
		public const ushort MinuteSecondMilFormat = 0x2f;		
		public const ushort RealFormat = 0x31;					
		#endregion
		public const ushort MaxRecSize97 = MaxBlockSize; 
		public const ushort BLIP_Extra = 17;
		public const ushort BLIP_Png   = 0x06E0;
		public const ushort BLIP_Jpeg   = 0x046A;
		public const ushort ObjRec_End = 0x00;
		public const ushort ObjRec_CF = 0x07;
		public const ushort ObjRec_PioGrbit = 0x08;
		public const ushort ObjRec_Cmo = 0x15;
		public const ushort MsoSpId = 0x0401;
		public const ushort SpId_Pict   = 0x4B;
		public const ushort MsoBLIP_Start = 0xF018;
		public const ushort MsoDggContainer = 0xF000;
		public const ushort MsoBStoreContainer = 0xF001;
		public const ushort MsoDgContainer = 0xF002;
		public const ushort MsoSpGrContainer = 0xF003;
		public const ushort MsoSpContainer = 0xF004;
		public const ushort MsoDgg = 0xF006;
		public const ushort MsoBse = 0xF007;
		public const ushort MsoDg = 0xF008;
		public const ushort MsoSpGr = 0xF009;
		public const ushort MsoSp = 0xF00A;
		public const ushort MsoOpt = 0xF00B;
		public const ushort MsoClientAnchor = 0xF010;
		public const ushort MsoClientData = 0xF011;
		public const ushort MsoSplitMenuColors = 0xF11E;
		public const ushort BIFFRecId_Hlink = 0x01B8;
		public const ushort BIFFRecId_MsoDrawingGroup = 0x00EB;
		public const ushort BIFFRecId_MsoDrawing = 0x00EC;
		public const ushort BIFFRecId_Continue =  0x003C;
		public const ushort BIFFRecId_Obj =  0x005D;
		public const int SizeOfBOF = 20;
		public const int SizeOfEOF = 4;
		public const int SizeOfWINDOW1 = 22;
		public const int SizeOfWINDOW2 = 22;
		public const int SizeOfFont_ = 30;
		public const int SizeOfTabID = 6;
		public const int SizeOfSupBook = 8;
		public const int SizeOfExternSheet = 12;
		public const int SizeOfDimension = 18;
		public const int SizeOfSTYLE = 48;  
		public const int SizeOfXF_ = CountOfXFStyles * 24;
		public const int SizeOfPalette_ = 56 * 4;
		public const int SizeOfHyperlinkData = 16;
		public const int DefaultDataSize = SizeOfBOF * 2 + SizeOfEOF * 2 +
			SizeOfWINDOW1 + SizeOfWINDOW2 + SizeOfFont_ * 5 + SizeOfTabID + 
			SizeOfSupBook + SizeOfExternSheet + SizeOfPalette_ + 6 + 
			SizeOfSTYLE + SizeOfDimension + 12 + SizeOfXF_;
		public static byte[] BOF = new byte[SizeOfBOF] { 
			 0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00, 0xBB, 0x0D,
			 0xCC, 0x07, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00};
		public static readonly byte[] EOF = new byte[SizeOfEOF] { 
			 0x0A, 0x00, 0x00, 0x00};
		public static readonly byte[] WINDOW1 = new byte[SizeOfWINDOW1] {
			 0x3D, 0x00, 0x12, 0x00, 0xE0, 0x01, 0x69, 0x00, 0xCC, 0x42, 0x7F,
			 0x26, 0x38, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x58, 0x02};
		public static byte[] WINDOW2 = new byte[SizeOfWINDOW2] {
			 0x3E, 0x02, 0x12, 0x00, 0xB6, 0x06, 0x00, 0x00, 0x00, 0x00, 0x40,
			 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}; 
		public static readonly byte[] Font_ = new byte[SizeOfFont_] {
			 0x31, 0x00, 0x1A, 0x00, 0xC8, 0x00, 0x00, 0x00, 0xFF, 0x7F, 0x90, 0x01, 0x00, 0x00, 0x00,
			 0x00, 0x00, 0x00, 0x05, 0x01, 0x41, 0x00, 0x72, 0x00, 0x69, 0x00, 0x61, 0x00, 0x6C, 0x00};
		public static readonly byte[] TabID = new byte[SizeOfTabID] {
			 0x3D, 0x01, 0x02, 0x00, 0x00, 0x00};
		public static readonly byte[] SupBook = new byte[SizeOfSupBook] {
			0xAE, 0x01, 0x04, 0x00, 0x01, 0x00, 0x01, 0x04};
		public static readonly byte[] ExternSheet = new byte[SizeOfExternSheet] {
			0x17, 0x00, 0x08, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
		public static byte[] Dimension = new byte[SizeOfDimension] {
			0x00, 0x02, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00};
		public static readonly byte[] STYLE = new byte[SizeOfSTYLE] {
			0x93, 0x02, 0x04, 0x00, 0x10, 0x80, 0x03, 0xFF,
			0x93, 0x02, 0x04, 0x00, 0x11, 0x80, 0x06, 0xFF,
			0x93, 0x02, 0x04, 0x00, 0x12, 0x80, 0x04, 0xFF,
			0x93, 0x02, 0x04, 0x00, 0x13, 0x80, 0x07, 0xFF,
			0x93, 0x02, 0x04, 0x00, 0x00, 0x80, 0x00, 0xFF,
			0x93, 0x02, 0x04, 0x00, 0x14, 0x80, 0x05, 0xFF};
		public static readonly byte[] XF_ = new byte[SizeOfXF_] {
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x01, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x01, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x02, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x02, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x20, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x01, 0x00, 0x2B, 0x00, 0xF8, 0xFF, 0x20, 0x00,
			0x00, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x01, 0x00, 0x29, 0x00, 0xF8, 0xFF, 0x20, 0x00,
			0x00, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x01, 0x00, 0x2C, 0x00, 0xF8, 0xFF, 0x20, 0x00,
			0x00, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x01, 0x00, 0x2A, 0x00, 0xF8, 0xFF, 0x20, 0x00,
			0x00, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x01, 0x00, 0x09, 0x00, 0xF8, 0xFF, 0x20, 0x00,
			0x00, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20};
		public static readonly int[] Palette_ = new int[SizeOfPalette_ / 4] {
			 0x000000, 0xFFFFFF, 0x0000FF, 0x00FF00, 0xFF0000, 0x00FFFF, 0xFF00FF, 0xFFFF00,
			 0x000080, 0x008000, 0x800000, 0x008080, 0x800080, 0x808000, 0xC0C0C0, 0x808080,
			 0xFF9999, 0x663399, 0xCCFFFF, 0xFFFFCC, 0x660066, 0x8080FF, 0xCC6600, 0xFFCCCC,
			 0x800000, 0xFF00FF, 0x00FFFF, 0xFFFF00, 0x800080, 0x000080, 0x808000, 0xFF0000,
			 0xFFCC00, 0xFFFFCC, 0xCCFFCC, 0x99FFFF, 0xFFCC99, 0xCC99FF, 0xFF99CC, 0x99CCFF,
			 0xFF6633, 0xCCCC33, 0x00CC99, 0x00CCFF, 0x0099FF, 0x0066FF, 0x996666, 0x969696,
			 0x663300, 0x669933, 0x003300, 0x003333, 0x003399, 0x663399, 0x993333, 0x333333};
		public static readonly byte[] HyperlinkData1 = new byte[SizeOfHyperlinkData] {
			0xD0, 0xC9, 0xEA, 0x79, 0xF9, 0xBA, 0xCE, 0x11, 
			0x8C, 0x82, 0x00, 0xAA, 0x00, 0x4B, 0xA9, 0x0B};
		public static readonly byte[] HyperlinkData2 = new byte[SizeOfHyperlinkData] {
			0xE0, 0xC9, 0xEA, 0x79, 0xF9, 0xBA, 0xCE, 0x11, 
			0x8C, 0x82, 0x00, 0xAA, 0x00, 0x4B, 0xA9, 0x0B};
	}
	#endregion
}
