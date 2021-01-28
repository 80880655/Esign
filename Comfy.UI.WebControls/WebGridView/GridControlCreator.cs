using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Comfy.UI.WebControls.WebGridView
{
    public class GridControlCreator
    {
        public static AbsControl CreateControl(string id, int columnSpan, int intWitch, Boolean isTime, Page page, Field Field,Boolean isSearch)
        {
            switch (Field.FieldType.ToString())
            {
                case "CheckBox":
                    return new GridCheckBox(id, columnSpan, intWitch, isTime, page, Field, isSearch);
                case "CheckBoxList":
                    return new GridCheckBoxList(id, columnSpan, intWitch, isTime, page, Field, isSearch);
                case "TextArea":
                    return new GridTextArea(id, columnSpan, intWitch, isTime, page, Field, isSearch);
                case "Date":
                    return new GridDate(id, columnSpan, intWitch, isTime, page, Field, isSearch);
                case "ComboBox":
                    return new GridComboBox(id, columnSpan, intWitch, isTime, page, Field, isSearch);
                case "ButtonEdit":
                    return new GridButtonEdit(id, columnSpan, intWitch, isTime, page, Field, isSearch);
                default:
                    return new GridDefaultControl(id, columnSpan, intWitch, isTime, page, Field, isSearch);

            }

        }
    }
}
