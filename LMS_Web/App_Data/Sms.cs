using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace LMS_Web.App_Data
{
    public class Sms
    {
        public void send(string mobilenumber, StringBuilder body)
        {
            string _URL = "http://139.162.49.213/api/push?"; //where the SMS Gateway is running
            string _user = HttpUtility.UrlEncode("pocadmin"); // API user name to send SMS
            string _pass = HttpUtility.UrlEncode("poc@123");
            string sender = HttpUtility.UrlEncode("poc@123");// API password to send SMS
            string _recipient = HttpUtility.UrlEncode(mobilenumber);  // who will receive message
            string _messageText = HttpUtility.UrlEncode(body.ToString()); // text message
            string _createURL = _URL +
           "user=" + _user +
           "&pwd=poc@123"+
           "&route=Transactional" +
           "&sender=POCBKN"+
           "&mobileno=" + _recipient +
           "&text=" + _messageText;
            try
            {

                HttpWebRequest _createRequest = (HttpWebRequest)WebRequest.Create(_createURL);
                HttpWebResponse myResp = (HttpWebResponse)_createRequest.GetResponse();
                System.IO.StreamReader _responseStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                string responseString = _responseStreamReader.ReadToEnd();
                _responseStreamReader.Close();
                myResp.Close();
            }
            catch(Exception e1)
            {
            }
        }
    }
}