using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using ZXing.Mobile;

namespace App1
{
    [Activity(Label = "License Scanner", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        string number;
        string dob;
        string issued;
        string expires;
        string name;
        string address;
        string city;
        string state;
        string zip;
        string sex;
        string height;
        string weight;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            SetContentView(Resource.Layout.Main);

            MobileBarcodeScanner.Initialize(Application);
            var options = new MobileBarcodeScanningOptions();
            options.PossibleFormats = new List<ZXing.BarcodeFormat>()
            {
                ZXing.BarcodeFormat.PDF_417
            };

            Button scanButton = FindViewById<Button>(Resource.Id.scanButton);
            TextView tvname = FindViewById<TextView>(Resource.Id.name);
            TextView tvlicense = FindViewById<TextView>(Resource.Id.license);
            TextView tvdob = FindViewById<TextView>(Resource.Id.dob);
            TextView tvissued = FindViewById<TextView>(Resource.Id.issued);
            TextView tvexpires = FindViewById<TextView>(Resource.Id.expires);
            TextView tvaddress = FindViewById<TextView>(Resource.Id.address);
            TextView tvsex = FindViewById<TextView>(Resource.Id.sex);
            TextView tvweight = FindViewById<TextView>(Resource.Id.hgt);
            TextView tvheight = FindViewById<TextView>(Resource.Id.wgt);

            scanButton.Click += async delegate
            {
                MobileBarcodeScanner scanner = new MobileBarcodeScanner();
                var result = await scanner.Scan(options);

                if (result != null)
                {
                    ParseData(result.Text);
                    tvname.Text = "Name: Unknown";
                    tvlicense.Text = "License Number: " + number;
                    tvdob.Text = "Date of Birth: " + dob;
                    tvissued.Text = "Date Issued: " + issued;
                    tvexpires.Text = "Date Expires: " + expires;
                    tvaddress.Text = "Address: " + address + ", " + city + ", " + state + ", " + zip;
                    tvsex.Text = "Sex: " + (sex == "M" ? "Male" : "Female");
                    tvweight.Text = "Weight: " + weight;
                    tvheight.Text = "Height: " + height;
                }
                else
                {
                    Console.WriteLine("scan is null");
                }
            };
            /*
            ANSI 6360350201DL00290182DLDAASMITH,KYLE,JAMES
            DAQS53051098370 // number
            DAB20200329 // expiration
            DBB19981229 // dob
            DAG3115 WALDEN LANE // address
            DAIWILMETTE // city
            DAJIL // state
            DAK600910000 // zip
            DARD // class
            DAS********  // rest
            DAT***** // end
            DBD20150502 // issued
            DBCM // sex
            DAU602 // height
            DAW150 // weight
            */
        }

        public void ParseData(string result)
        {
            Regex rnumber = new Regex("(DAQ).*");
            Regex rexpires = new Regex("(DBA).*");
            Regex rdob = new Regex("(DBB).*");
            Regex raddress = new Regex("(DAG).*");
            Regex rcity = new Regex("(DAI).*");
            Regex rstate = new Regex("(DAJ).*");
            Regex rzip = new Regex("(DAK).*");
            Regex rissued = new Regex("(DBD).*");
            Regex rsex = new Regex("(DBC).*");
            Regex rheht = new Regex("(DAU).*");
            Regex rweht = new Regex("(DAW).*");

            Match mnumber = rnumber.Match(result);
            if (mnumber.Success)
            {
                number = mnumber.Value.Substring(3);
            }

            Match mexpires = rexpires.Match(result);
            if(mexpires.Success)
            {
                var temp = mexpires.Value.Substring(3);
                var year = temp.Substring(0, 4);
                var month = temp.Substring(4, 2);
                var day = temp.Substring(6, 2);
                expires = string.Format("{0}-{1}-{2}", month, day, year);
            }

            Match mdob = rdob.Match(result);
            if (mdob.Success)
            {
                var temp = mdob.Value.Substring(3);
                var year = temp.Substring(0, 4);
                var month = temp.Substring(4, 2);
                var day = temp.Substring(6, 2);
                dob = string.Format("{0}-{1}-{2}", month, day, year);
            }

            Match maddress = raddress.Match(result);
            if(maddress.Success)
            {
                CultureInfo ci = Thread.CurrentThread.CurrentCulture;
                TextInfo ti = ci.TextInfo;

                string la = maddress.Value.Substring(3).ToLower();
                address = ti.ToTitleCase(la);
            }

            Match mcity = rcity.Match(result);
            if (mcity.Success)
            {
                CultureInfo ci = Thread.CurrentThread.CurrentCulture;
                TextInfo ti = ci.TextInfo;

                string lc = mcity.Value.Substring(3).ToLower();
                city = ti.ToTitleCase(lc);
            }

            Match mstate = rstate.Match(result);
            if(mstate.Success)
            {
                state = mstate.Value.Substring(3);
            }

            Match mzip = rzip.Match(result);
            if(mzip.Success)
            {
                zip = mzip.Value.Substring(3, mzip.Value.Length - mzip.Value.IndexOf("0000") - 1);
            }

            Match missued = rissued.Match(result);
            if(missued.Success)
            {
                var temp = missued.Value.Substring(3);
                var year = temp.Substring(0, 4);
                var month = temp.Substring(4, 2);
                var day = temp.Substring(6, 2);
                issued = string.Format("{0}-{1}-{2}", month, day, year);
            }

            Match msex = rsex.Match(result);
            if(msex.Success)
            {
                sex = msex.Value.Substring(3);
            }

            Match mweight = rweht.Match(result);
            if(mweight.Success)
            {
                weight = mweight.Value.Substring(3);
            }

            Match mheight = rheht.Match(result);
            if (mheight.Success)
            {
                var _height = mheight.Value.Substring(3);
                var feet = _height.Substring(0, 1);
                var inches = _height.Substring(1, 2);
                height = string.Format("{0}' {1}\"", feet, inches);
            }
        }
    }
}

