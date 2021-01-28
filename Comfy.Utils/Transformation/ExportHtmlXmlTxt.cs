

using System;
using System.Drawing;
using System.IO;
namespace Comfy.Utils.Transformation {
	public class ExportDefaultProvider: ExportCustomProvider, IExportProvider {
		protected ExportCacheItem[,] cache;
		protected int[] columns;
		protected int[] rows;
		protected ExportStyleManager styleManager;
		protected const string rangeError = "Incorrect cache width or height";
		protected const string indexError = "Incorrect cache col or row";
		protected const string unionError = "Incorrect union width or height";
		public ExportDefaultProvider(string fileName): base(fileName) {
			styleManager = ExportStyleManager.GetInstance(fileName, null);
		}
		public ExportDefaultProvider(Stream stream) : base(stream) {
			styleManager = ExportStyleManager.GetInstance("", stream);
		}
		void InternalSetRange(int width, int height, bool isVisible) {
			if(width <= 0 || height <= 0)
				throw new ExportCacheException(rangeError);
			cache = new ExportCacheItem[width, height];
			columns = new int[width];
			rows = new int[height];
			for(int i = 0; i < width; i++) {
				columns[i] = 0;
				for(int j = 0; j < height; j++) {
					cache[i, j].Data = null;
					cache[i, j].InternalCache = null;
					cache[i, j].IsHidden = false;
					cache[i, j].IsUnion = false;
					cache[i, j].UnionHeight = 1;
					cache[i, j].UnionWidth = 1;
					cache[i, j].StyleIndex = 0;
				}
			}
			for(int i = 0; i < height; i++)
				rows[i] = 0;
			ExportCacheCellStyle defaultStyle = styleManager.DefaultStyle;
			int defaultBorderWidth = 0;
			if(isVisible)
				defaultBorderWidth = 1;
			defaultStyle.LeftBorder.Width = defaultBorderWidth;
			defaultStyle.TopBorder.Width = defaultBorderWidth;
			defaultStyle.RightBorder.Width = defaultBorderWidth;
			defaultStyle.BottomBorder.Width = defaultBorderWidth;
			SetDefaultStyle(defaultStyle);
		}
		protected void TestIndex(int col, int row) {
			if(col < 0 || col >= CacheWidth() || 
				row < 0 || row >= CacheHeight())
				throw new ExportCacheException(indexError);		
		}
		protected int CacheWidth() {
			return cache.GetLength(0);
		}
		protected int CacheHeight() {
			return cache.GetLength(1);
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
		protected int GetCellWidth(int col, int row) {
			int cellWidth = 0;
			for(int i = 0; i < cache[col, row].UnionWidth; i++)
				cellWidth += GetColumnWidth(col + i);
			return cellWidth;
		}
		#region IExportProvider implementation
		public virtual void Commit() {
		}
		public int RegisterStyle(ExportCacheCellStyle style) {
			return styleManager.RegisterStyle(style);
		}
		public void SetDefaultStyle(ExportCacheCellStyle style) {
			styleManager.DefaultStyle = style;
		}
		public void SetStyle(ExportCacheCellStyle style) {
			int styleIndex = styleManager.RegisterStyle(style);
			SetStyle(styleIndex);
		}
		public void SetStyle(int styleIndex) {
			for(int i = 0; i < CacheWidth(); i++)
				for(int j = 0; j < CacheHeight(); j++)
					cache[i, j].StyleIndex = styleIndex;
		}
		public void SetCellStyle(int col, int row, int styleIndex) {
			TestIndex(col, row);
			cache[col, row].StyleIndex = styleIndex;
		}
		public void SetCellStyle(int col, int row, ExportCacheCellStyle style) {
			TestIndex(col, row);
			cache[col, row].StyleIndex = styleManager.RegisterStyle(style);
		}
		public void SetCellStyle(int col, int row, int exampleCol, int exampleRow) {
			TestIndex(col, row);
			TestIndex(exampleCol, exampleRow);
			cache[col, row].StyleIndex = cache[col, row].StyleIndex;
		}
		public void SetCellUnion(int col, int row, int width, int height) {
			if(width == 1 && height == 1) return;
			TestIndex(col, row);
			if(width <= 0 || (col + width - 1) >= CacheWidth() ||
				height <= 0 || (row + height - 1) >= CacheHeight())
				throw new ExportCacheException(unionError);
			cache[col, row].IsUnion = true;
			cache[col, row].IsHidden = false;
			cache[col, row].UnionWidth = width;
			cache[col, row].UnionHeight = height;
			for(int i = col; i < col + width; i++)
				for(int j = row; j < row + height; j++) {
					if(i == col && j == row)
						continue;
					cache[i, j].IsUnion = false;
					cache[i, j].IsHidden = true;
					cache[i, j].UnionWidth = 1;
					cache[i, j].UnionHeight = 1;
				}
		}
		public void SetCellStyleAndUnion(int col, int row, int width, int height, int styleIndex) {
			SetCellUnion(col, row, width, height);
			SetCellStyle(col, row, styleIndex);
		}
		public void SetCellStyleAndUnion(int col, int row, int width, int height, ExportCacheCellStyle style) {
			SetCellUnion(col, row, width, height);
			SetCellStyle(col, row, style);
		}
		public void SetRange(int width, int height, bool isVisible) {
			InternalSetRange(width, height, isVisible);
		}
		public void SetColumnWidth(int col, int width) {
			TestIndex(col, 0);
			columns[col] = width;
		}
		public void SetRowHeight(int row, int height) {
			TestIndex(0, row);
			rows[row] = height;
		}
		public void SetCellData(int col, int row, object data) {
			TestIndex(col, row);
			cache[col, row].Data = data;
			if(data is System.Boolean)
				cache[col, row].DataType = ExportCacheDataType.Boolean;
			else if(data is System.Int32)
				cache[col, row].DataType = ExportCacheDataType.Integer;
			else if(data is System.Single)
				cache[col, row].DataType = ExportCacheDataType.Single;
			else if(data is System.Double)
				cache[col, row].DataType = ExportCacheDataType.Double;
			else if(data is System.Decimal)
				cache[col, row].DataType = ExportCacheDataType.Decimal;
			else if(data is System.String)
				cache[col, row].DataType = ExportCacheDataType.String;
			else
				cache[col, row].DataType = ExportCacheDataType.Object;
		}
		public void SetCellString(int col, int row, string str) {
			TestIndex(col, row);
			cache[col, row].Data = str;
			cache[col, row].DataType = ExportCacheDataType.String;
		}
		public ExportCacheCellStyle GetStyle(int styleIndex) {
			return styleManager[styleIndex];
		}
		public ExportCacheCellStyle GetCellStyle(int col, int row) {
			TestIndex(col, row);
			return styleManager[cache[col, row].StyleIndex];
		}
		public ExportCacheCellStyle GetDefaultStyle() {
			return styleManager.DefaultStyle;
		}
		public int GetColumnWidth(int col) {
			TestIndex(col, 0);
			return columns[col];
		}
		public int GetRowHeight(int row) {
			TestIndex(0, row);
			return rows[row];
		}
		public virtual IExportProvider Clone(string fileName, Stream stream) {			
			return (IsStreamMode)? new ExportDefaultProvider(GetCloneStream(stream)) : new ExportDefaultProvider(GetCloneFileName(fileName));
		}
		#endregion
	}
	public class ExportDefaultInternalProvider: ExportDefaultProvider, IExportInternalProvider {
		public ExportDefaultInternalProvider(string fileName): base(fileName) {
		}
		public ExportDefaultInternalProvider(Stream stream) : base(stream) {
		}
		#region IExportInternalProvider implementation
		public virtual void CommitCache(StreamWriter writer) {
		}
		public void SetCacheToCell(int col, int row, IExportInternalProvider cache) {
			TestIndex(col, row);
			this.cache[col, row].InternalCache = cache;
		}
		public void DeleteCacheFromCell(int col, int row) {
			SetCacheToCell(col, row, null);
		}
		#endregion
		public override IExportProvider Clone(string fileName, Stream stream) {			
			return (IsStreamMode)? new ExportDefaultInternalProvider(GetCloneStream(stream)) : new ExportDefaultInternalProvider(GetCloneFileName(fileName));
		}
	}
	public class ExportHtmlProvider: ExportDefaultInternalProvider {
		public ExportHtmlProvider(string fileName): base(fileName) {
		}
		public ExportHtmlProvider(Stream stream) : base(stream) {
		}
		private bool IsEmptyString(string str) {
			for(int i = 0; i < str.Length; i++)
				if(str[i] != ' ' && str[i] != '\x0009')
					return false;
			return true;
		}
		private string ConvertCRLFSymbols(string text) {
			string result = "";
			for(int i = 0; i < text.Length; i++) {
				if(text[i] == '\x000A')
					result += "<br>";
				else
					result += text[i];
			}
			return result;
		}
		private string ConvertSpecialSymbols(string text) {
			string result = "";
			for(int i = 0; i < text.Length; i++) {
				if(text[i] == '<')
					result += "&lt;";
				else if(text[i] == '>')
					result += "&gt;";
				else if(text[i] == '&')
					result += "&amp;";
				else if(text[i] == '"')
					result += "&quot;";
				else
					result += text[i];
			}
			return result;
		}
		private string GetHtmlColor(Color color) {
			string result = "rgb(";
			result += Convert.ToString(color.R) + ',' +
				Convert.ToString(color.G) + ',' +
				Convert.ToString(color.B) + ')';
			return result;
		}
		private string GetStyle(ExportCacheCellStyle style) {
			string result = "";
			result += " border-style: solid;";
			result += " padding: 3;";
			if(style.LeftBorder.Width == style.TopBorder.Width &&
				style.LeftBorder.Width == style.RightBorder.Width &&
				style.LeftBorder.Width == style.BottomBorder.Width)
				result += " border-width: " + Convert.ToString(style.LeftBorder.Width) + ";";
			else {
				result += " border-left-width: " + Convert.ToString(style.LeftBorder.Width) + ";";
				result += " border-top-width: " + Convert.ToString(style.TopBorder.Width) + ";";
				result += " border-right-width: " + Convert.ToString(style.RightBorder.Width) + ";";
				result += " border-bottom-width: " + Convert.ToString(style.BottomBorder.Width) + ";";
			}
			if(style.LeftBorder.Color_ == style.TopBorder.Color_ &&
				style.LeftBorder.Color_ == style.RightBorder.Color_ &&
				style.LeftBorder.Color_ == style.BottomBorder.Color_)
				result += " border-color: "+ GetHtmlColor(style.LeftBorder.Color_) + ";";
			else {
				result += " border-left-color: " + GetHtmlColor(style.LeftBorder.Color_) + ";";
				result += " border-top-color: " + GetHtmlColor(style.TopBorder.Color_) + ";";
				result += " border-right-color: " + GetHtmlColor(style.RightBorder.Color_) + ";";
				result += " border-bottom-color: " + GetHtmlColor(style.BottomBorder.Color_) + ";";
			}
			result += " font-family: \"" + style.TextFont.Name + "\";";
			if(style.TextFont.Bold)
				result += " font-weight: bold;";
			if(style.TextFont.Italic)
				result += " font-style: italic;";
			if(style.TextFont.Underline)
				result += " text-decoration: underline;";
			if(style.TextFont.Strikeout)
				result += " text-decoration: line-through;";
			result += " font-size: " + Convert.ToString(style.TextFont.Size) + "pt;";
			result += " color: " + GetHtmlColor(style.TextColor) + ";";
			result += " background-color: " + GetHtmlColor(style.BkColor);
			return result;
		}
		private string GetTitle() {
			if(IsStreamMode) 
				return (Stream is FileStream)? Path.GetFileNameWithoutExtension(((FileStream)Stream).Name): ExportCustomProvider.StreamModeName;
			else
				return Path.GetFileNameWithoutExtension(FileName);
		}
		private void CommitStyles(StreamWriter writer) {
			writer.WriteLine("<style type=\"text/css\"><!--");
			for(int i = 0; i < styleManager.Count; i++) {
				writer.Write(".style" + Convert.ToString(i) + " {");
				writer.Write(GetStyle(styleManager[i]));
				writer.WriteLine("}");
			}
			writer.WriteLine("--></style>");
		}
		private void CommitHtml(StreamWriter writer) {
			writer.WriteLine("<html>");
			writer.WriteLine("<head>");
			writer.WriteLine("<title>" + GetTitle() + "</title>");
			writer.WriteLine("<META http-equiv=Content-type content=\"text/html; charset=UTF-8\">");
			CommitStyles(writer);
			writer.WriteLine("</head>");
			writer.WriteLine("<body>");
			CommitCache(writer);
			writer.WriteLine("</body>");
			writer.WriteLine("</html>");
		}
		public override void CommitCache(StreamWriter writer) {
			OnProviderProgress(0);
			writer.WriteLine("<table border=\"1\" cellspacing=\"0\"" +
				" cellpadding=\"0\">");
			for(int i = 0; i < CacheHeight(); i++) {
				writer.WriteLine("<tr>");
				for(int j = 0; j < CacheWidth(); j++) {
					if(cache[j, i].IsHidden)
						continue;
					if(cache[j, i].IsUnion) {
						writer.Write("<td");
						if(cache[j, i].UnionWidth > 1)
							writer.Write(" colspan=\"" + Convert.ToString(cache[j, i].UnionWidth) + "\"");
						if(cache[j, i].UnionHeight > 1)
							writer.Write(" rowspan=\"" + Convert.ToString(cache[j, i].UnionHeight) + "\"");
					} else
						writer.Write("<td");
					int cellWidth = GetCellWidth(j, i);
					if(cellWidth > 0)
						writer.Write(" width=\"" + Convert.ToString(cellWidth) + "\"");
					ExportCacheCellStyle style = GetCellStyle(j, i);
					writer.Write(" align=\"");					
					switch(style.TextAlignment) {
						case StringAlignment.Near:
							writer.Write("left");
							break;
						case StringAlignment.Center:
							writer.Write("center");
							break;
						case StringAlignment.Far:
							writer.Write("right");
							break;
					}
					writer.Write("\"");
					writer.Write(" nowrap");
					writer.Write(" class=\"style" + 
						Convert.ToString(cache[j, i].StyleIndex) + "\">");
					string displayValue = "";
					if(cache[j, i].InternalCache != null) 
						cache[j, i].InternalCache.CommitCache(writer);
					else {
						if(cache[j, i].Data == null)
							displayValue = "";
						else {
							switch(cache[j, i].DataType) {
								case ExportCacheDataType.String:
									displayValue = ConvertSpecialSymbols((string)cache[j, i].Data);
									break;
								case ExportCacheDataType.Integer:
									displayValue = Convert.ToString((int)cache[j, i].Data);
									break;
								case ExportCacheDataType.Single:
									displayValue = Convert.ToString((float)cache[j, i].Data);
									break;
								case ExportCacheDataType.Double:
									displayValue = Convert.ToString((double)cache[j, i].Data);
									break;
								case ExportCacheDataType.Boolean:
									displayValue = Convert.ToString((bool)cache[j, i].Data);
									break;
								case ExportCacheDataType.Decimal:
									displayValue = Convert.ToString((decimal)cache[j, i].Data);
									break;
								default:
									displayValue = Convert.ToString(cache[j, i].Data);
									break;
							}
							displayValue = ConvertCRLFSymbols(displayValue);
						}
						if(IsEmptyString(displayValue))
							displayValue += "&nbsp";
					}
					writer.WriteLine(displayValue + "</td>");
				}
				writer.WriteLine("</tr>");
				if(CacheHeight() > 1)
					OnProviderProgress(i * 100 / (CacheHeight() - 1));
			}
			writer.WriteLine("</table>");
			OnProviderProgress(100);
		}
		public override void Commit() {
			StreamWriter writer = (IsStreamMode)? new StreamWriter(Stream): new StreamWriter(FileName, false);
			try {
				CommitHtml(writer);
			}
			finally {
				if(IsStreamMode)
					writer.Flush();
				else
					writer.Close();
			}
		}
		public override IExportProvider Clone(string fileName, Stream stream) {			
			return (IsStreamMode)? new ExportHtmlProvider(GetCloneStream(stream)) : new ExportHtmlProvider(GetCloneFileName(fileName));
		}
	}
	public class ExportXmlProvider: ExportDefaultInternalProvider {
		private string xslFileName = "";
		public ExportXmlProvider(string fileName): base(fileName) {
		}
		public ExportXmlProvider(Stream stream) : base(stream) {
		}
		private string GetHtmlColor(Color color) {
			string result = "rgb(";
			result += Convert.ToString(color.R) + ',' +
				Convert.ToString(color.G) + ',' +
				Convert.ToString(color.B) + ')';
			return result;
		}
		private bool IsValidChar(char ch) {
			return 
				(ch >= 'a' && ch <= 'z') || 
				(ch >= 'A' && ch <= 'Z') ||
				(ch >= '0' && ch <= '9') || 
				(ch == '_') ||
				(ch == '-') || 
				(ch == ' ');
		}
		private bool IsEmptyString(string str) {
			for(int i = 0; i < str.Length; i++)
				if(str[i] != ' ' && str[i] != '\x0009')
					return false;
			return true;
		}
		private bool IsEmptyCharOnly(string str) {
			bool emptyCharUsed = false;
			for(int i = 0; i < str.Length; i++) {
				if(str[i] != ' ' && str[i] != '\x0009') {
					if(str[i] == EmptyChar) {
						if(!emptyCharUsed) 
							emptyCharUsed = true;
						else
							return false;
					} else
						return false;
				}
			}
			return true;
		}
		private string GetXslFileName(string xmlFileName) {
			string result = "";
			string name = Path.GetFileNameWithoutExtension(xmlFileName);
			string path = Path.GetDirectoryName(xmlFileName);
			if(path != "" && path[path.Length - 1] != '\\')
				path += '\\';
			for(int i = 0; i < name.Length; i++)
				if(IsValidChar(name[i]))
					result += name[i];
				else
					result += '_';
			result = path + result + ".xsl";
			return result;
		}
		private string GetAlignText(StringAlignment alignment) {
			switch(alignment) {
				case StringAlignment.Near:
					return "Left";
				case StringAlignment.Center:
					return "Center";
				case StringAlignment.Far:
					return "Right";
				default:
					return "";
			};
		}
		private string GetFontStyles(Font font) {
			string result = "";
			if(font.Bold)
				result += " bold=\"true\"";
			else
				result += " bold=\"false\"";
			if(font.Italic) 
				result += " italic=\"true\"";
			else
				result += " italic=\"false\"";
			if(font.Underline)
				result += " underline=\"true\"";
			else
				result += " underline=\"false\"";
			if(font.Strikeout)
				result += " strikeout=\"true\"";
			else
				result += " strikeout=\"false\"";
			return result;
		}
		private string GetStyle(ExportCacheCellStyle style) {
			string result = "";
			result += "aligntext=\"" + GetAlignText(style.TextAlignment) + "\"";
			result += " fontname=\"" + style.TextFont.Name + "\"";
			result += GetFontStyles(style.TextFont);
			result += " fontcolor=\"" + GetHtmlColor(style.TextColor) + "\"";
			result += " fontsize=\"" + Convert.ToString(style.TextFont.Size) + "\"";
			result += " bkcolor=\"" + GetHtmlColor(style.BkColor) + "\"";
			return result;
		}
		private string GetBorderStyleContent(ExportCacheCellBorderStyle borderStyle) {
			string result = "";
			result += " width=\"" + Convert.ToString(borderStyle.Width) + "\"";
			result += " color=\"" + GetHtmlColor(borderStyle.Color_) + "\"";
			return result;
		}
		private string GetBorderStyle(ExportCacheCellStyle style) {
			string result = "";
			result += "<border_left" + GetBorderStyleContent(style.LeftBorder) + "/>\n";
			result += "<border_up" + GetBorderStyleContent(style.TopBorder) + "/>\n";
			result += "<border_right" + GetBorderStyleContent(style.RightBorder) + "/>\n";
			result += "<border_down" + GetBorderStyleContent(style.BottomBorder) + "/>";
			return result;
		}
		private string GetCellParams(int col, int row) {
			string result = "";
			int width = GetCellWidth(col, row);
			if(width > 0)
				result += " width=\"" + Convert.ToString(width) + "\"";
			ExportCacheCellStyle style = GetCellStyle(col, row);
			result += " align=\"";
			switch(style.TextAlignment) {
				case StringAlignment.Near:
					result += "left\"";
					break;
				case StringAlignment.Center:
					result += "center\""; 
					break;
				case StringAlignment.Far:
					result += "right\"";
					break;
			};
			if(cache[col, row].IsUnion) {
				if(cache[col, row].UnionWidth > 1)
					result += " colspan=\"" + 
						Convert.ToString(cache[col, row].UnionWidth) + 
						"\"";
				if(cache[col, row].UnionHeight > 1)
					result += " rowspan=\"" + 
						Convert.ToString(cache[col, row].UnionHeight) + 
						"\"";
			}
			result += " styleclass=\"" + 
				Convert.ToString(cache[col, row].StyleIndex) +
				"\"";
			return result;
		}
		private string GetData(int col, int row) {
			string displayValue = "";
			if(cache[col, row].Data != null) {
				switch(cache[col, row].DataType) {
					case ExportCacheDataType.String:
						displayValue = (string)cache[col, row].Data;
						break;
					case ExportCacheDataType.Integer:
						displayValue = Convert.ToString((int)cache[col, row].Data);
						break;
					case ExportCacheDataType.Single:
						displayValue = Convert.ToString((float)cache[col, row].Data);
						break;
					case ExportCacheDataType.Double:
						displayValue = Convert.ToString((double)cache[col, row].Data);
						break;
					case ExportCacheDataType.Boolean:
						displayValue = Convert.ToString((bool)cache[col, row].Data);
						break;
					case ExportCacheDataType.Decimal:
						displayValue = Convert.ToString((decimal)cache[col, row].Data);
						break;
					default:
						displayValue = Convert.ToString(cache[col, row].Data);
						break;
				}
			}
			if(IsEmptyString(displayValue))
				displayValue += EmptyChar;
			return displayValue;
		}
		private string ConvertTextToXML(string text) {
			string result = "";
			for(int i = 0; i < text.Length; i++) {
				if(text[i] == '<')
					result += "&lt;";
				else if(text[i] == '>')
					result += "&gt;";
				else if(text[i] == '&') 
					result += "&amp;";
				else if(text[i] == '"')
					result += "&quot;";
				else if(text[i] == '\'')
					result += "&apos;";
				else
					result += text[i];
			}
			if(IsEmptyCharOnly(result))
				result = "&#160;"; 
			return result;
		}
		private string GetTitle() {
			if(IsStreamMode) 
				return (Stream is FileStream)? Path.GetFileNameWithoutExtension(((FileStream)Stream).Name): ExportCustomProvider.StreamModeName;
			else
				return Path.GetFileNameWithoutExtension(FileName);
		}
		private void CommitStyle(StreamWriter writer) {
			writer.WriteLine("<styles>");
			for(int i = 0; i < styleManager.Count; i++) {
				writer.WriteLine("<style id=\"" + Convert.ToString(i) +
					"\" " + GetStyle(styleManager[i]) +
					">");
				writer.WriteLine(GetBorderStyle(styleManager[i]));
				writer.WriteLine("</style>");
			}
			writer.WriteLine("</styles>");
		}
		private void CommitXml(StreamWriter writer) {
			writer.WriteLine("<?xml version=\"1.0\"?>");
			writer.WriteLine("<?xml-stylesheet type=\"text/xsl\" href=\"" + 
				Path.GetFileName(xslFileName) + "\"?>");
			writer.WriteLine("<cache>");
			writer.WriteLine("<title>" + GetTitle() + "</title>");
			CommitStyle(writer);
			CommitCache(writer);
			writer.WriteLine("</cache>");
		}
		private void CommitXsl(StreamWriter writer) {
			writer.WriteLine("<?xml version=\"1.0\"?>");
			writer.WriteLine("");
			writer.WriteLine("<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">");
			writer.WriteLine("<xsl:template match=\"/\">");
			writer.WriteLine("<xsl:apply-templates select=\"cache\" />");
			writer.WriteLine("</xsl:template>");
			writer.WriteLine("");
			writer.WriteLine("<xsl:template match=\"cache\">");
			writer.WriteLine("<html>");
			writer.WriteLine("<head>");
			writer.WriteLine("<xsl:apply-templates select=\"title\" />");
			writer.WriteLine("<xsl:apply-templates select=\"styles\" />");
			writer.WriteLine("</head>");
			writer.WriteLine("<body>");
			writer.WriteLine("<xsl:apply-templates select=\"lines\" />");
			writer.WriteLine("</body>");
			writer.WriteLine("</html>");
			writer.WriteLine("</xsl:template>");
			writer.WriteLine("");
			writer.WriteLine("<xsl:template match=\"title\">");
			writer.WriteLine("<title>");
			writer.WriteLine("<xsl:value-of select=\".\" />");
			writer.WriteLine("</title>");
			writer.WriteLine("</xsl:template>");
			writer.WriteLine("");
			writer.WriteLine("<xsl:template match=\"styles\">");
			writer.WriteLine("<style type=\"text/css\">");
			writer.WriteLine("<xsl:apply-templates select=\"style\" />");
			writer.WriteLine("</style>");
			writer.WriteLine("</xsl:template>");
			writer.WriteLine("");
			writer.WriteLine("<xsl:template match=\"style\">");
			writer.WriteLine(".style<xsl:value-of select=\"@id\" />");
			writer.WriteLine("{ border-style: solid; padding: 3;");
			writer.WriteLine("  font-family: <xsl:value-of select=\"@fontname\" />;");
			writer.WriteLine("  font-size: <xsl:value-of select=\"@fontsize\" />pt;");
			writer.WriteLine("  color: <xsl:value-of select=\"@fontcolor\" />;");
			writer.WriteLine("  background-color: <xsl:value-of select=\"@bkcolor\" />;");
			writer.WriteLine("<xsl:if test=\"@bold='true'\">");
			writer.WriteLine("  font-weight: bold;");
			writer.WriteLine("</xsl:if>");
			writer.WriteLine("<xsl:if test=\"@italic='true'\">");
			writer.WriteLine("  font-style: italic;");
			writer.WriteLine("</xsl:if>");
			writer.WriteLine("<xsl:if test=\"@underline='true'\">");
			writer.WriteLine("  text-decoration: underline;");
			writer.WriteLine("</xsl:if>");
			writer.WriteLine("<xsl:if test=\"@strikeout='true'\">");
			writer.WriteLine("  text-decoration: line-through;");
			writer.WriteLine("</xsl:if>");
			writer.WriteLine("<xsl:apply-templates select=\"border_left\" />");
			writer.WriteLine("<xsl:apply-templates select=\"border_up\" />");
			writer.WriteLine("<xsl:apply-templates select=\"border_right\" />");
			writer.WriteLine("<xsl:apply-templates select=\"border_down\" />");
			writer.WriteLine("}");
			writer.WriteLine("</xsl:template>");
			writer.WriteLine("");
			writer.WriteLine("<xsl:template match=\"border_left\">");
			writer.WriteLine("border-left-width: <xsl:value-of select=\"@width\" />;");
			writer.WriteLine("border-left-color: <xsl:value-of select=\"@color\" />;");
			writer.WriteLine("</xsl:template>");
			writer.WriteLine("");
			writer.WriteLine("<xsl:template match=\"border_up\">");
			writer.WriteLine("border-top-width: <xsl:value-of select=\"@width\" />;");
			writer.WriteLine("border-top-color: <xsl:value-of select=\"@color\" />;");
			writer.WriteLine("</xsl:template>");
			writer.WriteLine("");
			writer.WriteLine("<xsl:template match=\"border_right\">");
			writer.WriteLine("border-right-width: <xsl:value-of select=\"@width\" />;");
			writer.WriteLine("border-right-color: <xsl:value-of select=\"@color\" />;");
			writer.WriteLine("</xsl:template>");
			writer.WriteLine("");
			writer.WriteLine("<xsl:template match=\"border_down\">");
			writer.WriteLine("border-bottom-width: <xsl:value-of select=\"@width\" />;");
			writer.WriteLine("border-bottom-color: <xsl:value-of select=\"@color\" />;");
			writer.WriteLine("</xsl:template>");
			writer.WriteLine("");
			writer.WriteLine("<xsl:template match=\"lines\">");
			writer.WriteLine("<table border=\"1\" cellspacing=\"0\">");
			writer.WriteLine("<xsl:apply-templates select=\"line\" />");
			writer.WriteLine("</table>");
			writer.WriteLine("</xsl:template>");
			writer.WriteLine("");
			writer.WriteLine("<xsl:template match=\"line\">");
			writer.WriteLine("<tr>");
			writer.WriteLine("<xsl:apply-templates select=\"cell\" />");
			writer.WriteLine("</tr>");
			writer.WriteLine("</xsl:template>");
			writer.WriteLine("");
			writer.WriteLine("<xsl:template match=\"cell\">");
			writer.WriteLine("<td>");
			writer.WriteLine("<xsl:attribute name=\"nowrap\"></xsl:attribute>");
			writer.WriteLine("<xsl:attribute name=\"width\"><xsl:value-of select=\"@width\" /></xsl:attribute>");
			writer.WriteLine("<xsl:attribute name=\"align\"><xsl:value-of select=\"@align\" /></xsl:attribute>");
			writer.WriteLine("<xsl:attribute name=\"colspan\"><xsl:value-of select=\"@colspan\" /></xsl:attribute>");
			writer.WriteLine("<xsl:attribute name=\"rowspan\"><xsl:value-of select=\"@rowspan\" /></xsl:attribute>");
			writer.WriteLine("<xsl:attribute name=\"class\">style<xsl:value-of select=\"@styleclass\" /></xsl:attribute>");
			writer.WriteLine("<xsl:choose>");
			writer.WriteLine("<xsl:when test=\"lines\">");
			writer.WriteLine("<xsl:apply-templates select=\"lines\" />");
			writer.WriteLine("</xsl:when>");
			writer.WriteLine("<xsl:otherwise>");
			writer.WriteLine("<xsl:value-of select=\".\" />");
			writer.WriteLine("</xsl:otherwise>");
			writer.WriteLine("</xsl:choose>");
			writer.WriteLine("</td>");
			writer.WriteLine("</xsl:template>");
			writer.WriteLine("");
			writer.Write("</xsl:stylesheet>");
			OnProviderProgress(100);
		}
		private void CommitInStreamMode() {
			StreamWriter xmlWriter = new StreamWriter(Stream);
			try {
				CommitXml(xmlWriter);
			} finally {
				xmlWriter.Flush();
			}
		}
		private void CommitInFileMode() {
			xslFileName = GetXslFileName(FileName);
			StreamWriter xmlWriter = new StreamWriter(FileName, false);
			StreamWriter xslWriter = new StreamWriter(xslFileName, false);
			try {
				CommitXml(xmlWriter);
				CommitXsl(xslWriter);
			}
			finally {
				xslWriter.Close();
				xmlWriter.Close();
			}
		}
		public override void CommitCache(StreamWriter writer) {
			OnProviderProgress(0);
			writer.WriteLine("<lines colcount=\"" + Convert.ToString(CacheWidth()) +
				"\" rowcount=\"" + Convert.ToString(CacheHeight()) + "\">");
			for(int i = 0; i < CacheHeight(); i++) {
				writer.WriteLine("<line>");
				for(int j = 0; j < CacheWidth(); j++) {
					if(cache[j, i].IsHidden)
						continue;
					writer.Write("<cell" + GetCellParams(j, i) + ">");
					if(cache[j, i].InternalCache != null) 
						cache[j, i].InternalCache.CommitCache(writer);
					else
						writer.Write(ConvertTextToXML(GetData(j, i)));
					writer.WriteLine("</cell>");
				}
				writer.WriteLine("</line>");
				if(CacheHeight() > 1)
					OnProviderProgress(i * 99 / (CacheHeight() - 1));
			}
			writer.WriteLine("</lines>");
			OnProviderProgress(99);
		}
		public override void Commit() {
			if(IsStreamMode)
				CommitInStreamMode();
			else
				CommitInFileMode();
		}
		public override IExportProvider Clone(string fileName, Stream stream) {			
			return (IsStreamMode)? new ExportXmlProvider(GetCloneStream(stream)) : new ExportXmlProvider(GetCloneFileName(fileName));
		}
		public static char EmptyChar = '.';
	}
	public class ExportTxtProvider: ExportDefaultProvider {
		private string beginString = "";
		private string endString = "";
		private string separator = " ";
		private bool alignColumnWidth = true;
		private bool quoteData = false;
		private int[] columnMaxWidth;
		public ExportTxtProvider(string fileName): base(fileName) {
		}
		public ExportTxtProvider(Stream stream) : base(stream) {
		}
		private string GetData(int col, int row) {
			string displayValue = "";
			if(cache[col, row].Data != null) {
				switch(cache[col, row].DataType) {
					case ExportCacheDataType.String:
						displayValue = (string)cache[col, row].Data;
						break;
					case ExportCacheDataType.Integer:
						displayValue = Convert.ToString((int)cache[col, row].Data);
						break;
					case ExportCacheDataType.Single:
						displayValue = Convert.ToString((float)cache[col, row].Data);
						break;
					case ExportCacheDataType.Double:
						displayValue = Convert.ToString((double)cache[col, row].Data);
						break;
					case ExportCacheDataType.Boolean:
						displayValue = Convert.ToString((bool)cache[col, row].Data);
						break;
					case ExportCacheDataType.Decimal:
						displayValue = Convert.ToString((decimal)cache[col, row].Data);
						break;
					default:
						displayValue = Convert.ToString(cache[col, row].Data);
						break;
				}			
			}
			return displayValue;
		}
		private void CalculateColumnMaxWidth() {
			columnMaxWidth = new int[CacheWidth()];
			for(int i = 0; i < CacheWidth(); i++) {
				columnMaxWidth[i] = 0;
				for(int j = 0; j < CacheHeight(); j++) {
					if(cache[i, j].IsHidden)
						continue;
					string data = GetData(i, j); 
					if(data.Length > columnMaxWidth[i])
						columnMaxWidth[i] = data.Length;
				}
			}
		}
		private string AppendData(string data, int col) {
			if(columnMaxWidth[col] > data.Length) {
				int sub = columnMaxWidth[col] - data.Length;
				for(int i = 0; i < sub; i++)
					data += " ";
			}
			return data;
		}
		private string DoQuoteData(string data) {
			int index = 0;
			while(index < data.Length) {
				if(data[index] == '"') {
					data = data.Insert(index, "\"");
					index += 2;
				} else
					index++;
			}
			return "\"" + data + "\"";
		}
		string GetTextData(int i, int j) { 
			string data = GetData(j, i);
			if(quoteData)
				data = DoQuoteData(data);
			if(alignColumnWidth)
				data = AppendData(data, j);
			return data;
		}
		private void CommitTxt(StreamWriter writer) {
			OnProviderProgress(0);
			if(alignColumnWidth)
				CalculateColumnMaxWidth();
			for(int i = 0; i < CacheHeight(); i++) {
				writer.Write(beginString);
				int cacheWidth = CacheWidth();
				for(int j = 0; j < cacheWidth - 1; j++)
					writer.Write(GetTextData(i, j) + separator);
				writer.Write(GetTextData(i, cacheWidth - 1));
				writer.WriteLine(endString);
				if(CacheHeight() > 1)
					OnProviderProgress(i * 100 / (CacheHeight() - 1));
			}
			OnProviderProgress(100);
		}
		public override void Commit() {
			StreamWriter writer = (IsStreamMode)? new StreamWriter(Stream): new StreamWriter(FileName, false);
			try {
				CommitTxt(writer);
			}
			finally {
				if(IsStreamMode)
					writer.Flush();
				else
					writer.Close();
			}
		}
		public override IExportProvider Clone(string fileName, Stream stream) {
			ExportTxtProvider provider = (IsStreamMode)? new ExportTxtProvider(GetCloneStream(stream)) : new ExportTxtProvider(GetCloneFileName(fileName));
			provider.BeginString = beginString;
			provider.EndString = beginString;
			provider.Separator = separator;
			provider.AlignColumnWidth = alignColumnWidth;
			provider.QuoteData = quoteData;
			return provider;
		}
		public string BeginString {
			get {
				return beginString;
			}
			set {				
				beginString = value;
			}
		}
		public string EndString {
			get {
				return endString;
			}
			set {
				endString = value;
			}
		}
		public string Separator {
			get {
				return separator;
			}
			set {
				separator = value;
			}
		}
		public bool AlignColumnWidth {
			get {
				return alignColumnWidth;
			}
			set {
				alignColumnWidth = value;
			}
		}
		public bool QuoteData { 
			get { return quoteData; }
			set { quoteData = value; }
		}
	}
}
