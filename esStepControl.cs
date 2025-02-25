using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace StepControl
{
    [DefaultEvent("IndexChecked")]
    public partial class esStepControl : UserControl
    {
        public esStepControl()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.MouseDown += esStepControl_MouseDown;
        }

        /// <summary>
        /// 步骤更改事件
        /// </summary>
        [Description("步骤更改事件"), Category("自定义")]
        public event EventHandler IndexChecked;

        private Color m_stepBackColor = Color.FromArgb(189, 189, 189);
        /// <summary>
        /// 步骤背景色
        /// </summary>
        /// <value>The color of the step back.</value>
        [Description("步骤背景色"), Category("自定义")]
        public Color StepBackColor
        {
            get { return m_stepBackColor; }
            set
            {
                m_stepBackColor = value;
                Refresh();
            }
        }

        private Color m_stepForeColor = Color.FromArgb(255, 77, 59);
        /// <summary>
        /// 步骤前景色
        /// </summary>
        [Description("步骤前景色"), Category("自定义")]
        public Color StepForeColor
        {
            get { return m_stepForeColor; }
            set
            {
                m_stepForeColor = value;
                Refresh();
            }
        }

        private Color m_stepFontColor = Color.White;
        /// <summary>
        /// 步骤文字颜色
        /// </summary>
        [Description("步骤文字景色"), Category("自定义")]
        public Color StepFontColor
        {
            get { return m_stepFontColor; }
            set
            {
                m_stepFontColor = value;
                Refresh();
            }
        }

        private int m_stepWidth = 35;
        /// <summary>
        /// 步骤宽度
        /// </summary>
        [Description("步骤宽度景色"), Category("自定义")]
        public int StepWidth
        {
            get { return m_stepWidth; }
            set
            {
                m_stepWidth = value;
                Refresh();
            }
        }

        /// <summary>
		/// 默认3个
		/// </summary>
		private string[] m_steps = new string[] { "step1", "step2", "step3" };

        /// <summary>
        /// 步骤集合
        /// </summary>
        /// <value>The steps.</value>
        [Description("步骤"), Category("自定义")]
        public string[] Steps
        {
            get { return m_steps; }
            set
            {
                if (value == null || value.Length < 1)
                    return;
                m_steps = value;
                Refresh();
            }
        }

        private int m_stepIndex = 0;

        /// <summary>
        /// 获取、设置步骤位置
        /// </summary>
        /// <value>The index of the step.</value>
        [Description("步骤位置"), Category("自定义")]
        public int StepIndex
        {
            get { return m_stepIndex; }
            set
            {
                if (value > Steps.Length)
                    return;
                m_stepIndex = value;
                Refresh();
                if (IndexChecked != null)
                {
                    IndexChecked(this, null);
                }
            }
        }

        private int m_lineWidth = 2;

        /// <summary>
        /// 各个步骤之间的连接线宽度
        /// </summary>
        /// <value>The width of the line.</value>
        [Description("连接线宽度,最小2"), Category("自定义")]
        public int LineWidth
        {
            get { return m_lineWidth; }
            set
            {
                if (value < 2)
                    return;
                m_lineWidth = value;
                Refresh();
            }
        }

        private Image m_imgCompleted = null;
        /// <summary>
        /// 设置已完成步骤的图片24*24最完美
        /// </summary>
        /// <value>The img completed.</value>
        [Description("已完成步骤图片，当不为空时，已完成步骤将不再显示数字"), Category("自定义")]
        public Image ImgCompleted
        {
            get { return m_imgCompleted; }
            set
            {
                m_imgCompleted = value;
                Refresh();
            }
        }

        List<Rectangle> m_lstCacheRect = new List<Rectangle>();

        void esStepControl_MouseDown(object sender, MouseEventArgs e)
        {
            var index = m_lstCacheRect.FindIndex(p => p.Contains(e.Location));
            if (index >= 0)
            {
                StepIndex = index + 1;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;  //使绘图质量最高，即消除锯齿
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;

            if (m_steps != null && m_steps.Length > 0)
            {
                System.Drawing.SizeF sizeFirst = g.MeasureString(m_steps[0], this.Font);
                int y = (this.Height - m_stepWidth - 10 - (int)sizeFirst.Height) / 2;
                if (y < 0)
                    y = 0;

                int intTxtY = y + m_stepWidth + 10;
                int intLeft = 0;
                if (sizeFirst.Width > m_stepWidth)
                {
                    intLeft = (int)(sizeFirst.Width - m_stepWidth) / 2 + 1;
                }

                int intRight = 0;
                System.Drawing.SizeF sizeEnd = g.MeasureString(m_steps[m_steps.Length - 1], this.Font);
                if (sizeEnd.Width > m_stepWidth)
                {
                    intRight = (int)(sizeEnd.Width - m_stepWidth) / 2 + 1;
                }

                int intSplitWidth = 20;
                if (m_steps.Length > 1)
                {
                    intSplitWidth = (this.Width - m_steps.Length - (m_steps.Length * m_stepWidth) - intRight - intLeft) / (m_steps.Length - 1);
                }

                m_lstCacheRect = new List<Rectangle>();
                for (int i = 0; i < m_steps.Length; i++)
                {
                    #region 画圆，横线
                    Rectangle rectEllipse = new Rectangle(new Point(intLeft + i * (m_stepWidth + intSplitWidth), y), new Size(m_stepWidth, m_stepWidth));
                    m_lstCacheRect.Add(rectEllipse);
                    g.FillEllipse(new SolidBrush(m_stepBackColor), rectEllipse);

                    if (m_stepIndex > i)
                    {
                        g.FillEllipse(new SolidBrush(m_stepForeColor), new Rectangle(new Point(intLeft + i * (m_stepWidth + intSplitWidth), y), new Size(m_stepWidth, m_stepWidth)));

                    }
                    if (m_stepIndex > i && m_imgCompleted != null)
                    {
                        g.FillEllipse(new SolidBrush(Color.White), rectEllipse);
                        g.DrawImage(m_imgCompleted, new Rectangle(new Point((intLeft + i * (m_stepWidth + intSplitWidth) + (m_stepWidth - 30) / 2), y + (m_stepWidth - 30) / 2), new Size(32, 32)), 0, 0, m_imgCompleted.Width, m_imgCompleted.Height, GraphicsUnit.Pixel, null);
                    }
                    else
                    {
                        System.Drawing.SizeF _numSize = g.MeasureString((i + 1).ToString(), this.Font);
                        g.DrawString((i + 1).ToString(), Font, new SolidBrush(m_stepFontColor), new Point(intLeft + i * (m_stepWidth + intSplitWidth) + (m_stepWidth - (int)_numSize.Width) / 2 + 1, y + (m_stepWidth - (int)_numSize.Height) / 2 + 1));
                    }
                    #endregion

                    System.Drawing.SizeF sizeTxt = g.MeasureString(m_steps[i], this.Font);
                    g.DrawString(m_steps[i], Font, new SolidBrush(m_stepIndex > i ? m_stepForeColor : m_stepBackColor), new Point(intLeft + i * (m_stepWidth + intSplitWidth) + (m_stepWidth - (int)sizeTxt.Width) / 2 + 1, intTxtY));
                }

                for (int i = 0; i < m_steps.Length; i++)
                {
                    if (m_stepIndex > i)
                    {
                        if (i != m_steps.Length - 1)
                        {
                            if (m_stepIndex == i + 1)
                            {
                                g.DrawLine(new Pen(m_stepForeColor, m_lineWidth), new Point(intLeft + i * (m_stepWidth + intSplitWidth) + m_stepWidth - 3, y + ((m_stepWidth) / 2)), new Point(intLeft + i * (m_stepWidth + intSplitWidth) + m_stepWidth + intSplitWidth / 2, y + ((m_stepWidth) / 2)));
                                g.DrawLine(new Pen(m_stepBackColor, m_lineWidth), new Point(intLeft + i * (m_stepWidth + intSplitWidth) + m_stepWidth + intSplitWidth / 2, y + ((m_stepWidth) / 2)), new Point(intLeft + (i + 1) * (m_stepWidth + intSplitWidth) + 10, y + ((m_stepWidth) / 2)));
                            }
                            else
                            {
                                g.DrawLine(new Pen(m_stepForeColor, m_lineWidth), new Point(intLeft + i * (m_stepWidth + intSplitWidth) + m_stepWidth - 3, y + ((m_stepWidth) / 2)), new Point(intLeft + (i + 1) * (m_stepWidth + intSplitWidth) + 10, y + ((m_stepWidth) / 2)));
                            }
                        }
                    }
                    else
                    {
                        if (i != m_steps.Length - 1)
                        {
                            g.DrawLine(new Pen(m_stepBackColor, m_lineWidth), new Point(intLeft + i * (m_stepWidth + intSplitWidth) + m_stepWidth - 3, y + ((m_stepWidth) / 2)), new Point(intLeft + (i + 1) * (m_stepWidth + intSplitWidth) + 10, y + ((m_stepWidth) / 2)));
                        }
                    }
                }



            }

        }


    }
}
