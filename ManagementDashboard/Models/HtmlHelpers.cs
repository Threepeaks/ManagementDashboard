using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard
{
//            <div class="card">
//            <div class="card-body">
//                <div class="" style="overflow:auto;">
//                    <div id = "SubmissionReceivedTrend1" >< i class="fa fa-spinner fa-spin"></i> Loading</div>
//                </div>
//            </div>
//        </div>
    public  static class LayoutExtensions
    {

        public static IHtmlString CardWithTitleAndLoading(this HtmlHelper helper, string innerHolderId,string headerTitle)
        {
            TagBuilder card = new TagBuilder("div");
            card.Attributes.Add("class", "card");
            
            TagBuilder cardHeader = new TagBuilder("div");
            cardHeader.Attributes.Add("class", "card-header");
            
            TagBuilder cardTitle = new TagBuilder("div");
            cardTitle.Attributes.Add("class", "card-title");
            cardTitle.InnerHtml = headerTitle;
            cardHeader.InnerHtml = cardTitle.ToString();


            TagBuilder cardBody = new TagBuilder("div");
            cardBody.Attributes.Add("class", "card-body");

            TagBuilder holder = new TagBuilder("div");
            holder.Attributes.Add("id", innerHolderId);

            TagBuilder loadingIcon = new TagBuilder("i");
            loadingIcon.Attributes.Add("class", "fa fa-spinner fa-spin");

            holder.InnerHtml = loadingIcon.ToString() + " Loading";
            cardBody.InnerHtml = holder.ToString();
            card.InnerHtml = cardHeader.ToString() + cardBody.ToString();
            return new MvcHtmlString(card.ToString());

        }

        public static IHtmlString CardWithLoading(this HtmlHelper helper,string innerHolderId)
        {

            TagBuilder card = new TagBuilder("div");
            card.Attributes.Add("class", "card");

            TagBuilder cardBody = new TagBuilder("div");
            cardBody.Attributes.Add("class", "card-body");

            TagBuilder holder = new TagBuilder("div");
            holder.Attributes.Add("id", innerHolderId);

            TagBuilder loadingIcon = new TagBuilder("i");
            loadingIcon.Attributes.Add("class", "fa fa-spinner fa-spin");

            holder.InnerHtml = loadingIcon.ToString() + " Loading";
            cardBody.InnerHtml = holder.ToString();
            card.InnerHtml= cardBody.ToString();
            return new MvcHtmlString(card.ToString());
        }

    }
}