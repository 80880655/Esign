using System;

namespace Comfy.UI.WebControls.Transformation
{
	internal class FormatStringConverter {
		const double number = -123456789.987654321;
		const int noneDecimalNumber = 123456789;
		const string endFormatSymbol = "}";
		const string startFormatString = "{0:";
		public static FormatStringConverter CreateInstance(ushort preparedCellType, string formatStr) {
			string format = formatStr;			
			if(string.IsNullOrEmpty(format))
				return new FormatStringConverter(format);
			if(formatStr.IndexOf(startFormatString) != -1){
				int startIndex = formatStr.IndexOf(startFormatString) + 3;
				int length = formatStr.IndexOf(endFormatSymbol) - startIndex;
				format = formatStr.Substring(startIndex, length);
			}
			if(preparedCellType != XlsConsts.CurrencyNoneDecimalFormat && preparedCellType != XlsConsts.GeneralFormat)
				return new DateTimeFormatStringConverter(format);
			try {
				if(number.ToString(format) == format || number.ToString(format).IndexOf("1") == -1)
					return new DateTimeFormatStringConverter(format);
			}
			catch {
				return new DateTimeFormatStringConverter(format);
			}
			return new FormatStringConverter(format);
		}
		protected string formatString;
		public FormatStringConverter(string formatString) {
			this.formatString = formatString;
		}
		public virtual ushort GetCellType(ushort preparedCellType){
			return GetCellType();
		}
		protected virtual ushort GetCellType() {
			if(string.IsNullOrEmpty(formatString))
				return XlsConsts.GeneralFormat;
			if(CurrencyFormat())
				if(DecimalSeparated(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator))
					return XlsConsts.CurrencyDecimalFormat;
				else
					return XlsConsts.CurrencyNoneDecimalFormat;
			if(PercentFormat())
				if(DecimalSeparated(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.PercentDecimalSeparator))
					return XlsConsts.PercentDecimalFormat;
				else
					return XlsConsts.PercentNoneDecimalFormat;
			if(ExponentialFormat())
				return GetExponentialType();
			if(AccountFormat())
				if(DecimalSeparated(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator))
					return XlsConsts.AccountDecimalFormat;
				else
					return XlsConsts.AccountFormat;
			if(DigitSeparated())
				if(DecimalSeparated(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator))
					return XlsConsts.DigitDecimalFormat;
				else
					return XlsConsts.DigitNoneDecimalFormat;
			if(DecimalSeparated(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator))
				return XlsConsts.DecimalFormat;
			if(RealFormat(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator))
				return XlsConsts.RealFormat;
			return XlsConsts.NoneDecimalFormat;
		}
		bool DecimalSeparated(string decimalSeparator) {
			string formated = noneDecimalNumber.ToString(formatString);
			return formated.LastIndexOf(decimalSeparator) != -1;
		}
		bool DigitSeparated() {
			return number.ToString(formatString).IndexOf(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator) != -1;
		}
		bool CurrencyFormat() {
			string currencyString = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencySymbol;
			return number.ToString(formatString).IndexOf(currencyString) != -1 || formatString.IndexOf(currencyString) != -1;
		}
		bool PercentFormat() {
			return number.ToString(formatString).IndexOf(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.PercentSymbol) != -1;
		}
		bool ExponentialFormat() {
			string formated = number.ToString(formatString);
			return formated.IndexOf("E+") != -1 || formated.IndexOf("e+") != -1 || formated.IndexOf("E-") != -1 || formated.IndexOf("e-") != -1;
		}
		bool AccountFormat() {
			string formated = number.ToString(formatString);
			return formated.IndexOf("(") != -1 && formated.IndexOf(")") != -1;
		}
		bool RealFormat(string decimalSeparator) {
			string noneDecFormated = noneDecimalNumber.ToString(formatString);
			string decFormated = number.ToString(formatString);
			return decFormated.LastIndexOf(decimalSeparator) != -1 && noneDecFormated.LastIndexOf(decimalSeparator) == -1;
		}
		ushort GetExponentialType() {
			string formated = number.ToString(formatString);
			if(Math.Max(formated.IndexOf("e"), formated.IndexOf("E")) - formated.IndexOf(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator) < 3)
				return XlsConsts.ExponentalDecimalOneFormat;
			return XlsConsts.ExponentalDecimalFormat;
		}
	}
	internal class DateTimeFormatStringConverter : FormatStringConverter {
		public DateTimeFormatStringConverter(string formatString)
			: base(formatString) {
		}
		public override ushort GetCellType(ushort preparedCellType) {
			ushort result = GetCellType();
			return result == XlsConsts.GeneralFormat ? preparedCellType : result;
		}
		protected override ushort GetCellType() {
			if(string.IsNullOrEmpty(formatString))
				return XlsConsts.GeneralFormat;
			if(DateFormat() && TimeFormat())
				return XlsConsts.DateTimeFormat;
			if(TimeFormat())
				return GetTimeCellType();
			if(DateFormat())
				return GetDateCellType();
			return XlsConsts.DateTimeFormat;
		}
		ushort GetTimeCellType() {
			if(AMPMFormat()) {
				if(HaveSeconds())
					return XlsConsts.HourMinuteSecondAMPMFormat;
				return XlsConsts.HourMinuteAMPMFormat;
			}
			if(!HaveSeconds())
				return XlsConsts.HourMinuteFormat;
			if(HaveMillisecond())
				return XlsConsts.MinuteSecondMilFormat;
			if(HaveHours())
				return XlsConsts.HourMinuteSecondFormat;
			return XlsConsts.MinuteSecondFormat;
		}
		ushort GetDateCellType() {
			if(!HaveYear())
				return XlsConsts.DayMontnFormat;
			if(!HaveDay())
				return XlsConsts.MontnYearFormat;
			if(!StandartDateSeparate())
				return XlsConsts.DayMontnYearFormat;
			return XlsConsts.DateFormat;
		}
		bool DateFormat() {
			DateTime date = new DateTime(5, 5, 5, 1, 1, 1);
			return date.ToString(formatString).IndexOf("5") != -1 && date.ToString(formatString).IndexOf("1") == -1;
		}
		bool TimeFormat() {
			DateTime time = new DateTime(1, 1, 1, 5, 5, 5);
			return time.ToString(formatString).IndexOf("5") != -1 && time.ToString(formatString).IndexOf("1") == -1;
		}
		bool AMPMFormat() {
			if(string.IsNullOrEmpty(System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.AMDesignator))
				return false;
			DateTime time = new DateTime(1, 1, 1, 1, 0, 0);
			return time.ToString(formatString).IndexOf(System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.AMDesignator) != -1;
		}
		bool HaveHours() {
			DateTime time = new DateTime(1, 1, 1, 5, 1, 1);
			return time.ToString(formatString).IndexOf("5") != -1;
		}
		bool HaveSeconds() {
			DateTime time = new DateTime(1, 1, 1, 1, 1, 5);
			return time.ToString(formatString).IndexOf("5") != -1;
		}
		bool HaveMillisecond() {
			DateTime time = new DateTime(1, 1, 1, 1, 1, 1, 5);
			return time.ToString(formatString).IndexOf("5") != -1;
		}
		bool HaveYear() {
			DateTime date = new DateTime(55, 1, 1);
			return date.ToString(formatString).IndexOf("55") != -1;
		}
		bool HaveDay() {
			DateTime date = new DateTime(1, 1, 5);
			return date.ToString(formatString).IndexOf("5") != -1;
		}
		bool StandartDateSeparate() {
			DateTime date = new DateTime(1, 1, 1);
			return date.ToString(formatString).IndexOf(System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.DateSeparator) != -1;
		}
	}
}
