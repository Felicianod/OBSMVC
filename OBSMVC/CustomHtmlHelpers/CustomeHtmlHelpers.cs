//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


// 1. Create a Static Method in a Static Class 
// 2. The first Parameter has to be the type to which we are adding the extension method
// 3. Return Type should be "IHtmlString" as these strings are excluded from html encoding
// 4. To help with the creation of Html tags, use the 'TagBuilder' class
// 5. Include the namespace of the helper method in either the view or in web.config

namespace OBSMVC.CustomHtmlHelpers
{
    public static class CustomeHtmlHelpers
    {
        public static IHtmlString Image(this HtmlHelper helper, string src, string alt)
        {   //Can be invoked as:    @html.Image("Imagesourcepath", "alternateText")
            TagBuilder tb = new TagBuilder("img");
            tb.Attributes.Add("src", VirtualPathUtility.ToAbsolute(src));
            tb.Attributes.Add("alt", alt);
            return new MvcHtmlString(tb.ToString(TagRenderMode.SelfClosing));
        }
    }
}
