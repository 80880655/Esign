using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Comfy.UI.WebControls.WebButton
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ViewStateControl runat=server></{0}:ViewStateControl>")]
    public class ViewStateControl : WebControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Text"] = value;
            }
        }


        public string _text;
        public string Text_NoViewState
        {
            get {
                return _text;
            }
            set
            {
                this._text = value;
            }

        }

        public string Text_ViewState
        {
            get {
                string s = (string)ViewState["Text_ViewState"];
                return ((s==null)?string.Empty:s);
            }
            set
            {
                ViewState["Text_ViewState"] = value;
            }
        }


        public string Text_ViewStateOne
        {
            get
            {
                string s = (string)ViewState["Text_ViewStateOne"];
                return ((s == null) ? string.Empty : s);
            }
            set
            {
                ViewState["Text_ViewStateOne"] = value;
            }
        }

        private FaceStyle _faceStyle;
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        [Category("test")]
        [Description("自定義視圖狀態")]
        public FaceStyle FaceStyle
        {
            get {
                if (_faceStyle == null)
                {
                    _faceStyle = new FaceStyle();
                }
                if (IsTrackingViewState)
                {
                    (_faceStyle as IStateManager).TrackViewState();
                }
                return _faceStyle;
            }
        }

        protected override object SaveViewState()
        {
            Pair p = new Pair();
            p.First = base.SaveViewState();
            p.Second = (FaceStyle as IStateManager).SaveViewState();
            for (int i = 0; i < 2; i++)
            {
                if (p.First != null || p.Second != null)
                {
                    return p;
                }
            }
            return null;
        }


        protected override void LoadViewState(object savedState)
        {
            if (savedState == null)
            {
                base.LoadViewState(null);
                return;
            }
            else
            {
                Pair p = (Pair)savedState;
                if (p == null)
                {
                    throw new ArgumentException("無效的數據");
                }
                base.LoadViewState(p.First);
                if (p.Second != null)
                {
                    (FaceStyle as IStateManager).LoadViewState(p.Second);
                }
            }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            output.Write("XXXXXXXXXXXXXXX");
        }
    }
}
