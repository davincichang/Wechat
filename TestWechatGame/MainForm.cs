﻿using System;
using System.Windows.Forms;
using System.Data;

namespace TestWechatGame
{
    public partial class MainForm : Form
    {


        public MainForm()
        {
            InitializeComponent();
        }


        private Wechat.WeChatClient wechat = new Wechat.WeChatClient();
        private void MainForm_Load(object sender, EventArgs e)
        {
            comboBox_group.Enabled = false;
            button_run.Enabled = false;
            wechat.OnEvent += OnWechatAppEvent;
            wechat.Run();

        }


        private void OnWechatAppEvent(Wechat.WeChatClient sender, Wechat.WeChatClientEvent e)
        {

            RunInMainthread(() => {
                if (e is Wechat.GetQRCodeImageEvent)
                {
                    pictureBox_wechat.Image = (e as Wechat.GetQRCodeImageEvent).QRImage;
                }
                if (e is Wechat.UserScanQRCodeEvent)
                {
                    pictureBox_wechat.Image = (e as Wechat.UserScanQRCodeEvent).UserAvatarImage;
                }
                if (e is Wechat.LoginSucessEvent)
                {
                    Console.WriteLine("登陆成功！");
                }
                if (e is Wechat.AddMessageEvent)
                {
                    var message = (e as Wechat.AddMessageEvent).Msg;
                    Console.WriteLine("AddMsg:" + message.GetType());
                }
                if (e is Wechat.StatusChangedEvent)
                {
                    var changedEvent = (e as Wechat.StatusChangedEvent);
                    Console.WriteLine(string.Format( "StatusChanged {0} -> {1}",changedEvent.FromStatus,changedEvent.ToStatus));
                }
                if (e is Wechat.InitedEvent) {
                    ShowGroups();
                }
            });
        }


        private void ShowGroups()
        {
            var groups = wechat.Groups;
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("ID");
            foreach (var group in groups)
            {
                var row = dt.NewRow();
                row["Name"] = Utils.ClearHtml(group.NickName);
                row["ID"] = group.ID;
                dt.Rows.Add(row);
            }

            comboBox_group.DataSource = dt;
            comboBox_group.DisplayMember = "Name";
            comboBox_group.ValueMember = "ID";
            comboBox_group.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_group.Enabled = true;
            button_run.Enabled = true;
        }



        void RunAsync(Action action)
        {
            ((Action)(delegate () {
                action?.Invoke();
            })).BeginInvoke(null, null);
        }

        void RunInMainthread(Action action)
        {
            this.BeginInvoke((Action)(delegate () {
                action?.Invoke();
            }));
        }

        private void button_run_Click(object sender, EventArgs e)
        {
            //if (comboBox_group.SelectedValue != null)
            //{
            //    if (button_run.Text == "启动")
            //    {
            //        button_run.Text = "停止";
            //        app.RunGame(comboBox_group.SelectedValue as String);
            //    }
            //    else
            //    {
            //        button_run.Text = "启动";
            //        app.StopGame();
            //    }
            //}

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
