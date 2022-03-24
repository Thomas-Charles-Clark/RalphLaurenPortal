using RalphLaurenPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RalphLaurenPortal.ViewModels
{
    public class BaseViewModel
    {
        public string PageTitle { get; set; }
        public Member User { get; set; }
        public DateTime ContentDate { get; set; }

        public short CountryID 
        { 
            get
            {
                return User?.CountryID ?? 81;
            } 
        }

        public byte LanguageID 
        { 
            get
            {
                return User?.LanguageID ?? 1;
            } 
        }

        public string DotNetCulture 
        { 
            get
            {
                return User?.DotNetCulture ?? "en-GB";
            } 
        }

        public BaseViewModel(
            Member user,
            string pageTitle,
            DateTime? useDate)
        {
            User = user;
            PageTitle = pageTitle;
            ContentDate = useDate ?? DateTime.Now;
        }
    }
}