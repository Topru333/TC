using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        public TreeNode previousSelectedNode = null;
        private string tvselected;
        private string Tvselected//link с тривьюв
        {
            set { tvselected = value; }
                
            get { return tvselected;}
        }
        private string dvselected;//link с датагридвьюв
        public Form1()
        {
            InitializeComponent();
            GetDrives(DriveInfo.GetDrives());
            GetDrivesGrid(DriveInfo.GetDrives());
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Действие по нажатию/раскрытию ветви
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Сама ветвь</param>
        private void treeView1_NodeMouseClick(object sender, TreeViewEventArgs e)
        {
            try
            {
                e.Node.Nodes.Clear();
                DirectoryInfo directoryInfo;
                if (e.Node.FullPath.Length < 3)
                {
                    char t = e.Node.FullPath.ToString()[0];
                    directoryInfo = new DirectoryInfo(t + ":\\");
                }
                else { directoryInfo = new DirectoryInfo(e.Node.FullPath); }
                DirectoryInfo[] Directories = directoryInfo.GetDirectories();
                FileInfo[] files = directoryInfo.GetFiles();
                GetDirectories(Directories, e.Node, 1);
                for (int i = 0; i < e.Node.Nodes.Count; i++)
                {
                    e.Node.Nodes[i].Nodes.Add("");
                }
                GetFiles(directoryInfo, e.Node);
            }
            catch (Exception) { }
        }
        /// <summary>
        /// Действие по закрытию ветви
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Сама ветвь</param>
        private void treeView1_NodeMouseClick2(object sender, TreeViewEventArgs e)
        {
            try
            {
                DirectoryInfo directoryInfo;
                e.Node.Nodes.Clear();
                if(e.Node.FullPath.Length<3)
                {  directoryInfo = new DirectoryInfo(e.Node.FullPath + "/"); }
                else {  directoryInfo = new DirectoryInfo(e.Node.FullPath);}
                
                DirectoryInfo[] Directories = directoryInfo.GetDirectories();
                GetDirectories(Directories, e.Node, 1);
                GetFiles(directoryInfo, e.Node);

            }
            catch (Exception) { }
        }
        #region Тело поиска файлов и папок
        /// <summary>
        /// Получение списка "диски"
        /// </summary>
        /// <param name="Drives">Список всех дисков</param>
        private void GetDrives(DriveInfo[] Drives)
        {
            comboBox1.Items.Clear();
            if (Drives.Length > 0)
            {
                foreach (DriveInfo Drive in Drives)
                {
                    if (Drive.IsReady)
                    {
                        comboBox1.Items.Add(Lib.GetName(Drive));
                    }
                }
            }
        }
        private void GetDrivesGrid(DriveInfo[] Drives)
        {
            if (Drives.Length > 0)
            {
                flowLayoutPanel1.Controls.Clear();
                foreach(DriveInfo drive in Drives)
                {
                    Button button = new Button();
                    button.Name = drive.Name;
                    button.Text = drive.Name + " " + Lib.GetSize(drive);
                    button.Tag = drive;
                    button.Click += Disk_click;
                    flowLayoutPanel1.Controls.Add(button);
                }
                ChoiceDrivedv();
            }
        }
        /// <summary>
        /// Срабатывает по нажатию одного из дисков в Panel1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Disk_click(object sender, EventArgs e)
        {
            string path;
            ArrayList row;
            int i = 0;
            try
            {
                path = ((Button)sender).Name;
                LinkLabel.Text = path;
                dataGridView1.Rows.Clear();
                foreach (DirectoryInfo Directory in (new DirectoryInfo(path)).GetDirectories())
                {
                    row = new ArrayList();
                    row.Add(Directory);
                    row.Add("");
                    row.Add("Folder");
                    row.Add(imageList1.Images[1]);
                    dataGridView1.Rows.Add(row.ToArray());
                    dataGridView1.Rows[i].Tag = Directory.FullName;
                    i++;
                }
                foreach (FileInfo File in (new DirectoryInfo(path)).GetFiles())
                {
                    row = new ArrayList();
                    row.Add(File);
                    row.Add(Lib.GetSize(File));
                    row.Add("File" + "(" + File.Extension.ToString() + ")");
                    row.Add(imageList1.Images[2]);
                    dataGridView1.Rows.Add(row.ToArray());
                    dataGridView1.Rows[i].Tag = File.FullName;
                    i++;
                }
            }
            catch (Exception) { dataGridView1.Rows.Clear(); GetDrivesGrid(DriveInfo.GetDrives()); toolStripStatusLabel2.Text = " Error"; }
        }
        /// <summary>
        /// Получение папок и последующих файлов и папок
        /// </summary>
        /// <param name="Dirs">Массив из папок</param>
        /// <param name="nodeToAddTo">TreeView для добовления</param>
        /// <param name="Times">Сколько подпапок сканить (если число 2 то саму себя и ее подпапки)</param>
        private void GetDirectories(DirectoryInfo[] Dirs, TreeNode nodeToAddTo, int Times)
        {
            if (Dirs.Length > 0)
            {
                TreeNode aNode;
                DirectoryInfo[] SubDirs;
                foreach (DirectoryInfo subDir in Dirs)
                {
                    if (Lib.AccessToFolder(subDir))
                    {
                        aNode = new TreeNode(subDir.Name, 1, 3);
                        aNode.Tag = subDir;
                        aNode.ImageKey = "Folder";
                        SubDirs = subDir.GetDirectories();
                        Times--;
                        if (SubDirs.Length != 0 && Times > 0)
                        {
                            GetDirectories(SubDirs, aNode, Times);
                        }
                        aNode.ImageIndex = 1;
                        for (int i = 0; i < aNode.Nodes.Count; i++)
                        {
                            aNode.Nodes[i].Nodes.Add("");
                        }
                        GetFiles(subDir, aNode);
                        nodeToAddTo.Nodes.Add(aNode);

                    }
                }
            }
        }
        /// <summary>
        /// Получение файлов
        /// </summary>
        /// <param name="Directory">Папка из которой получаем файлы</param>
        /// <param name="nodeToAddTo">TreeView для добовления</param>
        private void GetFiles(DirectoryInfo Directory, TreeNode nodeToAddTo)
        {
            if (Directory.GetFiles().Length > 0)
            {
                TreeNode aNode;
                foreach (FileInfo File in Directory.GetFiles())
                {
                    if (File.Exists)
                    {
                        aNode = new TreeNode(Lib.GetName(File), 2, 3);
                        aNode.Tag = File;
                        aNode.ImageKey = "file";
                        aNode.ImageIndex = 2;
                        nodeToAddTo.Nodes.Add(aNode);
                    }
                }
            }
        }
        #endregion
        /// <summary>
        /// Обновляет список дисков
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Get_Drives_Click(object sender, EventArgs e)
        {
            GetDrives(DriveInfo.GetDrives());
            GetDrivesGrid(DriveInfo.GetDrives());
            ChoiceDrivedv();
            treeView1.Nodes.Clear();
            toolStripStatusLabel2.Text = " Got drives";
        }
        /// <summary>
        /// Функция кнопки удаления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, EventArgs e)
        {

            try
            {
                if (dataGridView1.Focused)
                {
                    foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
                    {
                        if (row.Cells[0].Value.GetType() == typeof(FileInfo))
                        {
                            File.Delete(row.Tag.ToString());
                            toolStripStatusLabel2.Text = " Deleted";
                        }
                        else if (row.Cells[0].Value.GetType() == typeof(DirectoryInfo))
                        {
                            Directory.Delete(row.Tag.ToString(),true);
                            toolStripStatusLabel2.Text = " Deleted";
                        }
                        else { toolStripStatusLabel2.Text = " Error"; }
                    }
                    dataGridView1.SelectedRows[0].Cells[0].Value = "Deleted";
                }
                else if (treeView1.Focused)
                {
                    if (treeView1.SelectedNode.Tag.GetType() == typeof(FileInfo))
                    {
                        File.Delete(treeView1.SelectedNode.FullPath.ToString());
                        toolStripStatusLabel2.Text = " Deleted";
                    }
                    else if (treeView1.SelectedNode.Tag.GetType() == typeof(DirectoryInfo))
                    {
                        Directory.Delete(treeView1.SelectedNode.FullPath.ToString(),true);
                        toolStripStatusLabel2.Text = " Deleted";
                    }
                    else { toolStripStatusLabel2.Text = " Error"; }
                }
                else { toolStripStatusLabel2.Text = " Error"; }
            }
            catch (Exception) { toolStripStatusLabel2.Text = " Error"; }
        }
        /// <summary>
        /// Действие по двойному клику на !первую! ячейку строчки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string path, oldPath;
                ArrayList row;
                int i = 0;
                try
                {
                    if (dataGridView1.SelectedRows[0].Cells[0].Value.GetType() == typeof(DirectoryInfo) || dataGridView1.SelectedRows[0].Cells[0].Value.GetType() == typeof(DriveInfo) || dataGridView1.SelectedRows[0].Cells[0].Value.GetType() == typeof(string))
                    {
                        path = dataGridView1.SelectedRows[0].Tag.ToString();
                        LinkLabel.Text = path;
                        dataGridView1.Rows.Clear();

                        
                        
                            oldPath = Path.GetDirectoryName(path);
                            row = new ArrayList();
                            row.Add("...");
                            row.Add("...");
                            row.Add("...");
                            row.Add(imageList1.Images[6]);
                            dataGridView1.Rows.Add(row.ToArray());
                            dataGridView1.Rows[i].Tag = oldPath;
                            i++;
                        
                        foreach (DirectoryInfo Directory in (new DirectoryInfo(path)).GetDirectories())
                        {
                            row = new ArrayList();
                            row.Add(Directory);
                            row.Add("");
                            row.Add("Folder");
                            row.Add(imageList1.Images[1]);
                            dataGridView1.Rows.Add(row.ToArray());
                            dataGridView1.Rows[i].Tag = Directory.FullName;
                            i++;
                        }
                        foreach (FileInfo File in (new DirectoryInfo(path)).GetFiles())
                        {
                            row = new ArrayList();
                            row.Add(File);
                            row.Add(Lib.GetSize(File));
                            row.Add("File" + "(" + File.Extension.ToString() + ")");
                            row.Add(imageList1.Images[2]);
                            dataGridView1.Rows.Add(row.ToArray());
                            dataGridView1.Rows[i].Tag = File.FullName;
                            i++;
                        }
                    }
                    else if (dataGridView1.SelectedRows[0].Cells[0].Value.GetType() == typeof(FileInfo))
                    {
                        path = dataGridView1.SelectedRows[0].Tag.ToString();
                        System.Diagnostics.Process.Start(path);
                        toolStripStatusLabel2.Text = " Opened";
                    }
                    else { toolStripStatusLabel2.Text = " Can't open"; }
                }
                catch (Exception)
                {
                    GetDrivesGrid(DriveInfo.GetDrives());
                    ChoiceDrivedv();
                }
            }
        }
        /// <summary>
        /// Добавление первоначального пункта "выберите диск"
        /// </summary>
        private void ChoiceDrivedv()
        {
            dataGridView1.Rows.Clear();
            ArrayList row = new ArrayList();
            row.Add("Choice drive");
            row.Add("...");
            row.Add("...");
            row.Add(imageList1.Images[4]);
            dataGridView1.Rows.Add(row.ToArray());
        }
        /// <summary>
        /// Действие по выбору диска в комбобоксе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            char driv = comboBox1.SelectedItem.ToString()[0];
            DriveInfo Drive = new DriveInfo(driv+":\\");
            TreeNode aNode;
            if (Drive.DriveType == DriveType.Removable)
            {
                aNode = new TreeNode(driv + ":", 5, 3);
            }
            else { aNode = new TreeNode(driv + ":", 4, 3); }
            
            GetDirectories((new DirectoryInfo(Drive.Name).GetDirectories()),aNode,1);
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(aNode);
        }
        /// <summary>
        /// Оба ниже срабатывают по переходу от одной панели к другой
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitContainer1_Panel1_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "1 is active ";
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Salmon;
            LinkLabel.BackColor = Color.Salmon;
            panel1.BackColor = Color.Salmon;
            splitContainer1.Panel1.BackColor = Color.Azure;
            splitContainer1.Panel2.BackColor = Color.LightGray;
        }
        private void splitContainer1_Panel2_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "2 is active ";
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.LightGray;
            LinkLabel.BackColor = Color.LightGray;
            panel1.BackColor = Color.LightGray;
            splitContainer1.Panel1.BackColor = Color.LightGray;
            splitContainer1.Panel2.BackColor = Color.Salmon;
        }
        /// <summary>
        /// Если двигается/не двигается центр то меняем курсор и наоборот
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitContainer1_SplitterMoving(Object sender, SplitterCancelEventArgs e)
        {
            Cursor.Current = System.Windows.Forms.Cursors.NoMoveHoriz;
        }
        private void splitContainer1_SplitterMoved(Object sender, SplitterEventArgs e)
        {
            Cursor.Current = System.Windows.Forms.Cursors.Default;
        }
        /// <summary>
        /// Функция кнопки копирования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Copy_Click(object sender, EventArgs e)
        {
            try
            {
                if (dvselected != "" && Tvselected != "")
                {
                    if (dataGridView1.Focused)
                    {
                        if (dataGridView1.SelectedRows[0].Cells[0].Value.GetType() == typeof(FileInfo))
                        {
                            if (Tvselected.Length < 3)
                            {
                                File.Copy(dvselected, Tvselected+"\\"+Path.GetFileName(dvselected));
                            }
                            else { File.Copy(dvselected, Tvselected+Path.GetFileName(dvselected)); }
                            toolStripStatusLabel2.Text = " Copied";
                        }
                        else if (dataGridView1.SelectedRows[0].Cells[0].Value.GetType() == typeof(DirectoryInfo))
                        {
                            Lib.DirectoryCopy(dvselected, Tvselected);
                            toolStripStatusLabel2.Text = " Copied";
                        }
                        else { toolStripStatusLabel2.Text = " Error"; }

                    }
                    else if (treeView1.Focused)
                    {
                        ArrayList row;
                        if (treeView1.SelectedNode.Tag.GetType() == typeof(FileInfo))
                        {
                            FileInfo file = new FileInfo(treeView1.SelectedNode.FullPath);
                            File.Copy(Tvselected, LinkLabel.Text);
                            toolStripStatusLabel2.Text = " Copied";
                            row = new ArrayList();
                            row.Add(treeView1.SelectedNode.Tag);
                            row.Add("");
                            row.Add("File" + "(" + file.Extension + ")");
                            row.Add(imageList1.Images[2]);
                            dataGridView1.Rows.Add(row.ToArray());
                            int i = dataGridView1.RowCount;
                            dataGridView1.Rows[i - 1].Tag = file.FullName;
                        }
                        else if (treeView1.SelectedNode.Tag.GetType() == typeof(DirectoryInfo))
                        {

                            Lib.DirectoryCopy(Tvselected, LinkLabel.Text);
                            DirectoryInfo dir = new DirectoryInfo(Path.Combine(LinkLabel.Text, new DirectoryInfo(Tvselected).Name));
                            toolStripStatusLabel2.Text = " Copied";
                            row = new ArrayList();
                            row.Add(new DirectoryInfo(Tvselected).Name);
                            row.Add("");
                            row.Add("Folder");
                            row.Add(imageList1.Images[1]);
                            dataGridView1.Rows.Add(row.ToArray());
                            int i = dataGridView1.RowCount;
                            dataGridView1.Rows[i-2].Tag = dir.FullName;
                        }
                        else { toolStripStatusLabel2.Text = " Error"; }
                    }
                    else { toolStripStatusLabel2.Text = " Error"; }
                }
                else { toolStripStatusLabel2.Text = " Choice links!"; }
            }
            catch (Exception e0) { toolStripStatusLabel2.Text = " Error" ; MessageBox.Show(e0.ToString()); }
        }
        /// <summary>
        /// Функция срабатывает после смены выбранного элемента (treeview1), сохраняет ссылку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                tvselected = treeView1.SelectedNode.FullPath;
                
            }
            catch (Exception) { return; }
            if (previousSelectedNode != null)
            {
                previousSelectedNode.BackColor = treeView1.BackColor;
                previousSelectedNode.ForeColor = treeView1.ForeColor;
            }
            try
            {
                treeView1.SelectedNode.BackColor = Color.Salmon;
                treeView1.SelectedNode.ForeColor = Color.White;
                previousSelectedNode = treeView1.SelectedNode;
            }
            catch (Exception) { }
        }
        /// <summary>
        /// Сохранение линка(dataGridView1)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                dvselected = dataGridView1.SelectedRows[0].Tag.ToString();
            }
            catch (Exception) { return; }
        }
    }
}