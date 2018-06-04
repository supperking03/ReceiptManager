using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace InHoaDonBaCang
{
    public partial class lbHoaDon : Form
    {
        public class convertNumberToString
        {
            public static string ChuyenSo(string number)
            {
                if (number.Contains(".")) return "khong biet";
                number = number.Replace("đồng", "");
                number = number.Replace(" ", "");

                string[] dv = { "", "mươi", "trăm", "nghìn", "triệu", "tỉ" };
                string[] cs = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
                string doc;
                int i, j, k, n, len, found, ddv, rd;

                len = number.Length;
                number += "ss";
                doc = "";
                found = 0;
                ddv = 0;
                rd = 0;

                i = 0;
                while (i < len)
                {
                    //So chu so o hang dang duyet
                    n = (len - i + 2) % 3 + 1;
                    //Kiem tra so 0
                    found = 0;
                    for (j = 0; j < n; j++)
                    {
                        if (number[i + j] != '0')
                        {
                            found = 1;
                            break;
                        }
                    }
                    //Duyet n chu so
                    if (found == 1)
                    {
                        rd = 1;
                        for (j = 0; j < n; j++)
                        {
                            ddv = 1;
                            switch (number[i + j])
                            {
                                case '0':
                                    if (n - j == 3) doc += cs[0] + " ";
                                    if (n - j == 2)
                                    {
                                        if (number[i + j + 1] != '0') doc += "lẻ ";
                                        ddv = 0;
                                    }
                                    break;
                                case '1':
                                    if (n - j == 3) doc += cs[1] + " ";
                                    if (n - j == 2)
                                    {
                                        doc += "mười ";
                                        ddv = 0;
                                    }
                                    if (n - j == 1)
                                    {
                                        if (i + j == 0) k = 0;
                                        else k = i + j - 1;

                                        if (number[k] != '1' && number[k] != '0')
                                            doc += "mốt ";
                                        else
                                            doc += cs[1] + " ";
                                    }
                                    break;
                                case '5':
                                    if (i + j == len - 1)
                                        doc += "lăm ";
                                    else
                                        doc += cs[5] + " ";
                                    break;
                                default:
                                    doc += cs[(int)number[i + j] - 48] + " ";
                                    break;
                            }

                            //Doc don vi nho
                            if (ddv == 1)
                            {
                                doc += dv[n - j - 1] + " ";
                            }
                        }
                    }
                    //Doc don vi lon
                    if (len - i - n > 0)
                    {
                        if ((len - i - n) % 9 == 0)
                        {
                            if (rd == 1)
                                for (k = 0; k < (len - i - n) / 9; k++)
                                    doc += "tỉ ";
                            rd = 0;
                        }
                        else
                            if (found != 0) doc += dv[((len - i - n + 1) % 9) / 3 + 2] + " ";
                    }
                    i += n;
                }

                if (len == 1)
                    if (number[0] == '0' || number[0] == '5') return cs[(int)number[0] - 48];
                return doc + " đồng";
            }
        }
        public lbHoaDon()
        {
            try
            {
                InitializeComponent();
                this.StartPosition = FormStartPosition.CenterScreen;
                this.Size = new Size(1000, Screen.PrimaryScreen.WorkingArea.Height);
                colThanhTien.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                colDonGia.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                colSoLuong.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                colTenHang.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                txtNgayHoaDon.Text = DateTime.Today.ToShortDateString();
                txtNgayHoaDon.TextAlign = HorizontalAlignment.Center;

                gridNhapHoaDon.EditingControlShowing += eventNhapVaoDataGrid;
                gridNhapHoaDon.CellEnter += eventLeave;
                txtTongThanhTien.TextChanged += new EventHandler(txtTongThanhTien_TextChanged);
                btnIn.Click += new EventHandler(btnIn_Clicked);
                btnNhapPhieuMoi.Click += new EventHandler(btnNhapPhieuMoi_Clicked);
            }
            catch { }
        }



        #region xu ly thanh cong

        private void eventNhapVaoDataGrid(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(colSoLuong_KeyPress);
            e.Control.KeyPress -= new KeyPressEventHandler(colDonGia_KeyPress);

            if (gridNhapHoaDon.CurrentCell.ColumnIndex == 3) //Desired Column
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(colSoLuong_KeyPress);
                }
            }
            if (gridNhapHoaDon.CurrentCell.ColumnIndex == 4)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(colDonGia_KeyPress);
                    tb.TextChanged += new EventHandler(colDonGia_TextChanged);
                }
            }

        }
        private void colDonGia_TextChanged(object sender, EventArgs e)
        {
            lastRowIndex = gridNhapHoaDon.CurrentCell.RowIndex;
        }
        private void colDonGia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar == 46)
            {
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }
        }
        private void colSoLuong_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void btnNhapPhieuMoi_Clicked(object sender, EventArgs e)
        {
            gridNhapHoaDon.Rows.Clear();
            txtSoHoaDon.Text = txtDiaChi.Text = txtSoDienThoai.Text = txtTongThanhTien.Text = txtChuyenThanhChu.Text = txtHoTen.Text = "";
        }

        private void txtTongThanhTien_TextChanged(object sender, EventArgs e)
        {
            txtChuyenThanhChu.Text = convertNumberToString.ChuyenSo(txtTongThanhTien.Text);
        }




        #region xu ly thanh tien va thong so null
        private string designStringNumber(string number)
        {
            number = number.Replace(" ", "");
            int i = number.Length - 1;
            int dem = 1;
            while (i != 0)
            {
                if (dem == 3)
                {
                    dem = 0;
                    number = number.Insert(i, " ");
                }
                dem += 1;
                i--;
            }
            return number;
        }

        int lastRowIndex = -1;


        private void eventLeave(object sender, DataGridViewCellEventArgs e)
        {
            double doubleTongThanhTien = 0;
            //update Stt
            for (int i = 0; i <= gridNhapHoaDon.Rows.Count - 2; i++)
                gridNhapHoaDon.Rows[i].Cells[0].Value = i + 1;

            //update thanh tien, tong thanh tien
            for (int i = 0; i <= gridNhapHoaDon.Rows.Count - 1; i++)
            {
                if (gridNhapHoaDon.Rows[i].Cells[4].Value == null || gridNhapHoaDon.Rows[i].Cells[4].Value.ToString() == "" | gridNhapHoaDon.Rows[i].Cells[4].Value == DBNull.Value)
                {
                    gridNhapHoaDon.Rows[i].Cells[4].Value = 0;
                }
                if (gridNhapHoaDon.Rows[i].Cells[3].Value == null || gridNhapHoaDon.Rows[i].Cells[3].Value.ToString() == "" || gridNhapHoaDon.Rows[i].Cells[3].Value == DBNull.Value)
                {
                    gridNhapHoaDon.Rows[i].Cells[3].Value = 1;
                }
                //bat dau tinh thanh tien
                gridNhapHoaDon.Rows[i].Cells[3].Value = gridNhapHoaDon.Rows[i].Cells[3].Value.ToString().Replace(" ", "");
                gridNhapHoaDon.Rows[i].Cells[4].Value = gridNhapHoaDon.Rows[i].Cells[4].Value.ToString().Replace(" ", "");

                double doubleDongia = double.Parse(gridNhapHoaDon.Rows[i].Cells[4].Value.ToString().Replace(".", ","));

                //chuyen doi ra thanh kieu don vi tinh ngan dong
                if (i == lastRowIndex)
                {
                    doubleDongia *= 1000;
                }

                double intSoluong = double.Parse(gridNhapHoaDon.Rows[i].Cells[3].Value.ToString());
                double doubleThanhTien = doubleDongia * intSoluong;

                doubleTongThanhTien += doubleThanhTien;

                gridNhapHoaDon.Rows[i].Cells[4].Value = designStringNumber(doubleDongia.ToString());
                gridNhapHoaDon.Rows[i].Cells[5].Value = designStringNumber(doubleThanhTien.ToString());
            }
            lastRowIndex = -1;
            txtTongThanhTien.Text = designStringNumber((doubleTongThanhTien).ToString()) + "    đồng";
        }
        #endregion


        protected override void OnLoad(EventArgs e)
        {
            gridNhapHoaDon.Columns[2].Visible = false;
        }
        #endregion
        private void btnIn_Clicked(object sender, EventArgs e)
        {
            if (gridNhapHoaDon.Rows.Count<=1)
                MessageBox.Show("Bạn chưa nhập mặt hàng nào cả");
            else
            {
                try
                {
                    PrintDialog _PrintDialog = new PrintDialog();
                    PrintDocument _PrintDocument = new PrintDocument();

                    _PrintDialog.Document = _PrintDocument; //add the document to the dialog box
                    _PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(_CreateReceipt); //add an event handler that will do the printing
                                                                                                                   //on a till you will not want to ask the user where to print but this is fine for the test envoironment.
                    //DialogResult result = _PrintDialog.ShowDialog();
                    _PrintDocument.DocumentName = "BaCang.pdf";

                   // if (result == DialogResult.OK)
                    //{
                    _PrintDocument.Print();
                    //}
                }
                catch (Exception ecr)
                {
                    MessageBox.Show(ecr.ToString());
                }

            }
        }



        private void _CreateReceipt(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics graphic = e.Graphics;
            Font font = new Font("Courier New", 9);
            Font smallfont = new Font("Courier New", 8);
            Font largefont = new Font("Courier New", 10,FontStyle.Bold);

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            float FontHeight = font.GetHeight();
            int startX = 5;
            int startY = 30;
            // int offset = 30;
            //for (int i = 0; i <= 500; i++)
            //{
            //    graphic.DrawString(((char)170).ToString(), smallfont, new SolidBrush(Color.Black), 275, i);
            //}


            //Introduce Vuon hoa kieng ba cang
            graphic.DrawString("                      ", new Font("Courier New", 12, FontStyle.Underline), new SolidBrush(Color.Black), 30, startY);
            startY += 10;
            graphic.DrawString(".                      ", new Font("Courier New", 12, FontStyle.Underline), new SolidBrush(Color.Black), 30, startY);
            startY += 20;
            graphic.DrawString("Vườn Hoa Kiểng BÁ CANG", new Font("Courier New", 12, FontStyle.Underline), new SolidBrush(Color.Black), 30, startY);

            Bitmap original = (Bitmap)Image.FromFile("imgCay.png");
            Bitmap resized = new Bitmap(original, new Size(original.Width / 20, original.Height / 20));
            Image imgBaCang = (Image)resized;

            int startXofIntroduce = startX + 95;
            startY += 30;

            graphic.DrawImage(imgBaCang, 5, startY - 7);
                        graphic.DrawString("    Hoa Kiểng, BonSai", smallfont, new SolidBrush(Color.Black), startXofIntroduce, startY);
                      graphic.DrawString("\n     Cây trồng, Chậu", smallfont, new SolidBrush(Color.Black), startXofIntroduce, startY);
                    graphic.DrawString("\n\n   Phân, Thuốc Các loại", smallfont, new SolidBrush(Color.Black), startXofIntroduce, startY);
                graphic.DrawString("\n\n\n\n     Nhận: Bảo dưỡng,", smallfont, new SolidBrush(Color.Black), startXofIntroduce, startY);
              graphic.DrawString("\n\n\n\n\n    Chăm sóc sân vườn", smallfont, new SolidBrush(Color.Black), startXofIntroduce, startY);
            graphic.DrawString("\n\n\n\n\n\nChỉnh sửa Bonsai cao cấp", smallfont, new SolidBrush(Color.Black), startXofIntroduce, startY);
            startY += 5;
               graphic.DrawString("\n\n\n\n\n\n\n\n     358A Bình Long, p.Phú Thọ Hòa", smallfont, new SolidBrush(Color.Black), 5, startY);

             graphic.DrawString("\n\n\n\n\n\n\n\n\n       q.Tân Phú, tp.Hồ Chí Minh", smallfont, new SolidBrush(Color.Black), 5, startY);
           graphic.DrawString("\n\n\n\n\n\n\n\n\n\n     ĐT: 08.38606121 - DĐ: 0938444010", smallfont, new SolidBrush(Color.Black), 5, startY);

            startY = startY + (int)smallfont.GetHeight() * 10 + 20;
            startY += 10;
            graphic.DrawString("\t\tHÓA ĐƠN",largefont, new SolidBrush(Color.Black), startX, startY);


            startY += 20;
            graphic.DrawString("          ----------------------", smallfont, new SolidBrush(Color.Black), startX, startY);
            startY = startY + 20;
            graphic.DrawString("Ngày: " + txtNgayHoaDon.Text, smallfont, new SolidBrush(Color.Black), 5, startY);
            graphic.DrawString("\nĐơn số: " + txtSoHoaDon.Text, smallfont, new SolidBrush(Color.Black), 5, startY);
            graphic.DrawString("\n\nKhách hàng: " + txtHoTen.Text, smallfont, new SolidBrush(Color.Black), 5, startY);
            int numberOfXuongHang = 0;
            


            if (txtDiaChi.Text != null && txtDiaChi.Text!="")
            {
                string strHoTen = txtDiaChi.Text;
                List<string> chuoiHoTen = new List<string> { };
                while (strHoTen.Length >= 30)
                {
                    string smallTen = "";
                    int index = 30;
                    if (strHoTen.Substring(0, 30).Contains(" "))
                        index = strHoTen.Substring(0, 30).LastIndexOf(' ');
                    smallTen = strHoTen.Substring(0, index);
                    strHoTen = strHoTen.Substring(index, strHoTen.Length - index);
                    chuoiHoTen.Add(smallTen);
                }
                if (strHoTen.Length > 0)
                    chuoiHoTen.Add(strHoTen);
                graphic.DrawString("\n\n\nĐC : " + chuoiHoTen[0], smallfont, new SolidBrush(Color.Black), 5, startY);
                if (chuoiHoTen.Count > 1)
                {
                    for (int i = 1; i <= chuoiHoTen.Count - 1; i++)
                    {
                        graphic.DrawString("\n\n\n           " + strXuongDong(i - 1) + chuoiHoTen[i], smallfont, new SolidBrush(Color.Black), 5, startY);
                        numberOfXuongHang = i;
                    }
                }
            }
            if (txtDiaChi.Text=="")
            {
                graphic.DrawString(strXuongDong(numberOfXuongHang) + "\n\nĐT: " + txtSoDienThoai.Text, smallfont, new SolidBrush(Color.Black), 5, startY);
            }
            else
            graphic.DrawString(strXuongDong(numberOfXuongHang) + "\n\n\nĐT: " + txtSoDienThoai.Text, smallfont, new SolidBrush(Color.Black), 5, startY);
            startY = startY + (int)smallfont.GetHeight() * (4+numberOfXuongHang) + 20;
            graphic.DrawString("          ----------------------", smallfont, new SolidBrush(Color.Black), 5, startY);

            //datagrid view cua hoa don

            startY += 20;
            string top = "STT".PadRight(3) + "   TÊN HÀNG".PadRight(18) + "SL".PadRight(4) + "ĐƠN GIÁ";
            graphic.DrawString(top, font, new SolidBrush(Color.Black), startX, startY);
            graphic.DrawString("---------------------------------------------------------------------------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + 10);
            startY += 20;

            numberOfXuongHang = 0;

            for (int i = 0; i <= gridNhapHoaDon.Rows.Count - 2; i++)
            {
                string strStt = "";
                string strTenHang = "";
                //string strDvt = "";
                string strSoLuong = "";
                string strDonGia = "";

                if (gridNhapHoaDon.Rows[i].Cells[0].Value != null)
                    strStt = gridNhapHoaDon.Rows[i].Cells[0].Value.ToString();

                if (gridNhapHoaDon.Rows[i].Cells[1].Value != null)
                {
                    numberOfXuongHang += i;
                    strTenHang = gridNhapHoaDon.Rows[i].Cells[1].Value.ToString();
                    List<string> chuoiTenHang = new List<string> { };

                    while (strTenHang.Length >= 18)
                    {
                        string smallTen = "";
                        int index = 18;

                        if (strTenHang.Substring(0, 18).Contains(" "))
                            index = strTenHang.Substring(0, 18).LastIndexOf(' ');
                        smallTen = strTenHang.Substring(0, index);
                        strTenHang = strTenHang.Substring(index, strTenHang.Length - index);
                        chuoiTenHang.Add(smallTen);
                    }
                    if (strTenHang.Length > 0)
                        chuoiTenHang.Add(strTenHang);

                    //if (gridNhapHoaDon.Rows[i].Cells[2].Value != null)
                    //    strDvt = gridNhapHoaDon.Rows[i].Cells[2].Value.ToString();

                    if (gridNhapHoaDon.Rows[i].Cells[3].Value != null)
                        strSoLuong = gridNhapHoaDon.Rows[i].Cells[3].Value.ToString();

                    if (gridNhapHoaDon.Rows[i].Cells[4].Value != null)
                        strDonGia = gridNhapHoaDon.Rows[i].Cells[4].Value.ToString().Replace(" ", ",");

                    //  if (gridNhapHoaDon.Rows[i].Cells[5].Value != null)
                    //     strThanhTien = gridNhapHoaDon.Rows[i].Cells[5].Value.ToString().Replace(" ", ",");

                    string strItem = strStt.PadRight(3) + chuoiTenHang[0].PadRight(18) + strSoLuong.PadRight(4) + strDonGia;

                    strItem = strXuongDong(numberOfXuongHang) + strItem;

                    graphic.DrawString(strItem, font, new SolidBrush(Color.Black), startX, startY);


                    if (chuoiTenHang.Count > 1)
                    {
                        numberOfXuongHang += 1;

                        for (int j = 1; j <= chuoiTenHang.Count - 1; j++)
                        {
                            graphic.DrawString(strXuongDong(numberOfXuongHang) + "".PadRight(3) + chuoiTenHang[j].PadRight(18), font, new SolidBrush(Color.Black), startX, startY);
                            numberOfXuongHang += 1;
                        }
                    }
                }
            }
            startY = startY + numberOfXuongHang * (int)font.GetHeight() + 50;
            graphic.DrawString("          ----------------------", smallfont, new SolidBrush(Color.Black), 5, startY);

            startY += 20;

            string numberTongCong = txtTongThanhTien.Text.Replace("đồng", "");
            numberTongCong = numberTongCong.TrimEnd();
            numberTongCong = numberTongCong.Replace(" ", ",");
            graphic.DrawString("Tổng: " + numberTongCong + " đ", largefont, new SolidBrush(Color.Black), 5, startY);
            startY += 10;

            string strChuSo = txtChuyenThanhChu.Text;

            List<string> chuoiChu = new List<string> { };
            while (strChuSo.Length >= 30)
            {
                string smallTen = "";
                int index = 30;

                if (strChuSo.Substring(0, 30).Contains(" "))
                    index = strChuSo.Substring(0, 30).LastIndexOf(' ');
                smallTen = strChuSo.Substring(0, index);
                strChuSo = strChuSo.Substring(index, strChuSo.Length - index);
                chuoiChu.Add(smallTen);
            }
            if (strChuSo.Length > 0)
                chuoiChu.Add(strChuSo);

            numberOfXuongHang = 0;
            graphic.DrawString("\nChữ : " + chuoiChu[0], font, new SolidBrush(Color.Black), 5, startY);
            if (chuoiChu.Count > 1)
            {

                for (int i = 1; i <= chuoiChu.Count - 1; i++)
                {
                    graphic.DrawString("\n             " + strXuongDong(i - 1) + chuoiChu[i], font, new SolidBrush(Color.Black), 5, startY);
                    numberOfXuongHang = i;
                }
            }

            startY = startY + numberOfXuongHang * (int)font.GetHeight() + 50;

            Font fontKiten = new Font("Courier New", 8,FontStyle.Underline);

            graphic.DrawString("          ----------------------", smallfont, new SolidBrush(Color.Black), 5, startY);
            startY += 20;
            graphic.DrawString("Người Nhận Hàng",fontKiten, new SolidBrush(Color.Black), 5, startY);
            graphic.DrawString("Người Viết Hóa Đơn", fontKiten, new SolidBrush(Color.Black), 140, startY);

            startY += 100;
            graphic.DrawString("Đã Nhận Đủ Tiền", fontKiten, new SolidBrush(Color.Black), 5, startY );
            startY += 100;
            graphic.DrawString(".", smallfont, new SolidBrush(Color.Black), 5, startY);




        }
        private string strXuongDong(int sodong)
        {
            string chuoi = "";
            if (sodong >= 0)
            {
                for (int i = 0; i <= sodong; i++)
                    chuoi += '\n';
            }
            return chuoi;
        }

    }
}