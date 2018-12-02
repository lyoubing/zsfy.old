using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Analysis.Work.Execute
{
    public partial class WorkHandler : Form
    {
        public WorkHandler()
        {
            InitializeComponent();
        }
        System.Timers.Timer timerCount = new System.Timers.Timer();
        System.Timers.Timer timerReportMonitor = new System.Timers.Timer();
        //电生理报告
        System.Timers.Timer timerEctReportMonitor = new System.Timers.Timer();
        protected override void OnLoad(EventArgs e)
        {

            timerCount.Enabled = true;
            timerCount.Interval =30000;//执行间隔时间,单位为毫秒  
            timerCount.Start();
            timerCount.Elapsed += new System.Timers.ElapsedEventHandler(Timer1_Elapsed);
            //base.OnLoad(e);

            timerReportMonitor.Enabled = true;
            timerReportMonitor.Interval = 3000;//执行间隔时间,单位为毫秒  
            timerReportMonitor.Start();
            timerReportMonitor.Elapsed += new System.Timers.ElapsedEventHandler(timerReportMonitor_Elasped);
            //base.OnLoad(e);

            timerEctReportMonitor.Enabled = true;
            timerEctReportMonitor.Interval = 30000;//执行间隔时间,单位为毫秒  
            timerEctReportMonitor.Start();
            timerEctReportMonitor.Elapsed += new System.Timers.ElapsedEventHandler(timerEctReportMonitor_Elasped);
            //base.OnLoad(e);
        }

        bool ectBusy = false;
        private void timerEctReportMonitor_Elasped(object sender, System.Timers.ElapsedEventArgs e)
        {
           // throw new NotImplementedException();
            if (!ectBusy)
            {
                ectBusy = true;
                try
                {
                    worker.HandleEctPrint();
                }
                catch { }
                ectBusy = false;
            }           
        }

      


        NetScape.AnalysisWork.Work.Analysis worker = new NetScape.AnalysisWork.Work.Analysis();
        private void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
           // throw new NotImplementedException();
            if (!isBusy)
            {
                isBusy = true;
                try
                {
                    worker.DOWork();
                }
                catch { }
                isBusy = false;
            }            
           
        }

        bool isBusy = false;

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();          
        }

        private void WorkHandler_Load(object sender, EventArgs e)
        {

        }

        bool MonitorBusy = false;
        /// <summary>
        /// 监视打印目录，如果生成Hl7消息，则取其pdf报告，修改格式后输出到指定目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerReportMonitor_Elasped(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!MonitorBusy)
            {
                MonitorBusy = true;
                //加个保险，以免意想不到的异常
                try
                {
                    worker.HandleGEPrint();
                }
                catch { }
                MonitorBusy = false;
            } 
        }
    }
}
