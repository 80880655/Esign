using System;
using System.Collections.Generic;
using System.Text;

namespace Comfy.UI.WebControls.WebGridView
{
    public class TextJs
    {

        public static string Js = @"
<script type='text/javascript'>
//服務器處理完之後調用的函數

var $tableId$ = new WebGridView('$tableId$', $addForm$, $searchForm$);

</script>
";

    }
}
