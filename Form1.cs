using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CMP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //1. Listeyi Oku
            //2. Tweetleri kelimelere böl TweetListe ekle aynısından varsa countu arttır

        }

        public class TopTweet
        {
            public string Keyword { get; set; }
            public int Count { get; set; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            char[] delimiterChars = { ' ', ',', '.', ':' };
            List<TopTweet> TopTweetList = new List<TopTweet>();
            int TweetTrue = 0;

            using (var reader = new StreamReader(@"training.1600000.processed.noemoticon.csv"))
            {
                List<string> listA = new List<string>();
                List<string> listB = new List<string>();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    TweetItem tweet = TweetSplit(line);

                    //önce user kontrolü
                    TweetTrue = TopTweetList.FindIndex(x => x.Keyword == tweet.User);
                    //Yoksa listeye ekle
                    if (TweetTrue == -1)
                    {
                        TopTweet topTweet = new TopTweet();
                        topTweet.Keyword = tweet.User;
                        topTweet.Count = 1;
                        TopTweetList.Add(topTweet);
                    }
                    //Varsa Sayısını Arttır
                    if (TweetTrue != -1)
                    {
                        TopTweet topTweet = TopTweetList.Find(x => x.Keyword == tweet.User);
                        topTweet.Count += 1;
                        //Eskisini silip
                        TopTweetList.RemoveAt(TweetTrue);
                        //Sayısı artan halini ekliyoruz
                        TopTweetList.Insert(TweetTrue, topTweet);

                    }

                    //Sonra tweeti kelimelere bölüp kontrol

                    string[] TweetTextList = tweet.Tweet.Split(delimiterChars);
                    for (int index = 0; index < TweetTextList.Length; index++)
                    {
                        //önce user kontrolü
                        TweetTrue = TopTweetList.FindIndex(x => x.Keyword == TweetTextList[index]);
                        //Yoksa listeye ekle
                        if (TweetTrue == -1)
                        {
                            TopTweet topTweet = new TopTweet();
                            topTweet.Keyword = TweetTextList[index];
                            topTweet.Count = 1;
                            TopTweetList.Add(topTweet);
                        }
                        //Varsa Sayısını Arttır
                        if (TweetTrue != -1)
                        {
                            TopTweet topTweet = TopTweetList.Find(x => x.Keyword == TweetTextList[index]);
                            topTweet.Count += 1;
                            //Eskisini silip
                            TopTweetList.RemoveAt(TweetTrue);
                            //Sayısı artan halini ekliyoruz
                            TopTweetList.Insert(TweetTrue, topTweet);

                        }
                    }

                }

            }

            //Sıralama            
            TopTweetList.Sort(delegate (TopTweet x, TopTweet y)
            {
                return y.Count.CompareTo(x.Count);
            });

            //Yazdırma
            int TopLimit = 0;
            foreach (var item in TopTweetList)
            {
                listBox1.Items.Add(item.Keyword + "(" + item.Count + ")");
                TopLimit++;
                if (TopLimit > 10)
                    break;
            }
            

        }


        //Custom Split
        public class TweetItem
        {
            public int Polority { get; set; }
            public int Id { get; set; }
            public string Date { get; set; }
            public string Query { get; set; }
            public string User { get; set; }
            public string Tweet { get; set; }
        }

        public TweetItem TweetSplit(string TweetLine)
        {
            TweetItem tweetItem = new TweetItem();
            Char Control;
            int CountSeparator = 0;
            string Text = "";

            for (int index = 0; index < TweetLine.Length; index++)
            {
                Control = Convert.ToChar(TweetLine.Substring(index, 1));

                // 5 tane , bulduysan bundan sonrası tweet , aramaya gerek yok.
                // tırnaları almasın diye başına +1 sonuna +2 koydum
                if (CountSeparator == 5)
                {
                    tweetItem.Tweet = TweetLine.Substring(index + 1, TweetLine.Length - (index + 2));
                    return tweetItem;
                }

                // , olana kadar karakterleri text yap
                if (Control != 44)
                {
                    Text += Control;
                }
                // , olduğunda sıraya göre ilgili itema texti yaz
                // Dosyada Her itemın yeri belli olduğu için bu yöntem uygun
                if (Control == 44)
                {
                    if (CountSeparator == 0)
                        tweetItem.Polority = Convert.ToInt32(ClearText(Text));
                    if (CountSeparator == 1)
                        tweetItem.Id = Convert.ToInt32(ClearText(Text));
                    if (CountSeparator == 2)
                        tweetItem.Date = ClearText(Text);
                    if (CountSeparator == 3)
                        tweetItem.Query = ClearText(Text);
                    if (CountSeparator == 4)
                        tweetItem.User = ClearText(Text);

                    CountSeparator++;
                    Text = "";
                }


            }

            return tweetItem;
        }

        public string ClearText(string Text)
        {
            return Text.Replace("\"", "");
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //-------------
    }
}
