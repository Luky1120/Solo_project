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


namespace Exploer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //현재 로컬 컴퓨터에 존재하는 드라이브 정보 검색하여 트리노드에 추가
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo dname in allDrives)
            {
                if (dname.DriveType == DriveType.Fixed)
                {
                    if (dname.Name == @"C:\")
                    {
                        TreeNode rootNode = new TreeNode(dname.Name);
                        rootNode.ImageIndex = 0;
                        rootNode.SelectedImageIndex = 0;
                        treeView1.Nodes.Add(rootNode);
                        NodePlus(rootNode);
                    }
                    else
                    {
                        TreeNode rootNode = new TreeNode(dname.Name);
                        rootNode.ImageIndex = 1;
                        rootNode.SelectedImageIndex = 1;
                        treeView1.Nodes.Add(rootNode);
                        NodePlus(rootNode);
                    }
                }
            }

            //첫번째 노드 확장
            treeView1.Nodes[0].Expand();

            //ListView 보기 속성 설정
          
            listView1.View = View.Details;

            //ListView Details 속성을 위한 헤더 추가
            listView1.Columns.Add("이름", listView1.Width / 3, HorizontalAlignment.Left);
            listView1.Columns.Add("유형", listView1.Width / 3, HorizontalAlignment.Left);
            listView1.Columns.Add("크기", listView1.Width / 3, HorizontalAlignment.Left);

            //행 단위 선택 가능
            listView1.FullRowSelect = true;
        }

        private void NodePlus(TreeNode dirNode)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(dirNode.FullPath);
                //드라이브의 하위 폴더 추가
                foreach (DirectoryInfo dirItem in dir.GetDirectories())
                {
                    TreeNode newNode = new TreeNode(dirItem.Name);
                    newNode.ImageIndex = 2;
                    newNode.SelectedImageIndex = 2;
                    dirNode.Nodes.Add(newNode);
                    newNode.Nodes.Add("*");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("에러 발생");
            }
        }

        // 트리가 닫히기 전에 발생하는 이벤트
        private void treeView1_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes[0].Text == "*")
            {
                e.Node.ImageIndex = 2;
                e.Node.SelectedImageIndex = 2;
            }
        }

        // 트리가 확장되기 전에 발생하는 이벤트
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes[0].Text == "*")
            {
                e.Node.Nodes.Clear();
                e.Node.ImageIndex = 3;
                e.Node.SelectedImageIndex = 3;
                NodePlus(e.Node);
            }
        }

        //노드를 클릭했을 때 ListView에 파일목록 띄우기
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            SettingListView(e.Node.FullPath);
        }


        public void SettingListView(string sFullPath)
        {
            try
            {
                //기존의 파일 목록 제거
                listView1.Items.Clear();
                //현재 경로를 표시
                string str = sFullPath;
                textBox2.Text = str.Substring(0, 2)+"\\"+str.Substring(4);  
                DirectoryInfo dir = new DirectoryInfo(sFullPath);
                int DirectCount = 0;
                //하위 디렉토리 보기
                foreach (DirectoryInfo dirItem in dir.GetDirectories())
                {
                    //하위 디렉토리가 존재할 경우 ListView에 추가
                    //ListViewItem 객체를 생성
                    ListViewItem lsvitem = new ListViewItem();
                    //생성된 ListViewItem 객체에 똑같은 이미지를 할당
                    lsvitem.ImageIndex = 2;
                    lsvitem.Text = dirItem.Name;
                    //아이템을 (listView1)에 추가
                    listView1.Items.Add(lsvitem);
                    listView1.Items[DirectCount].SubItems.Add("폴더");
                    listView1.Items[DirectCount].SubItems.Add(dirItem.GetFiles().Length.ToString()
                        + " files");
                    DirectCount++;
                }


                //디렉토리에 존재하는 파일목록 및 확장자 크기 보여주기
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo fileinfo in files)
                {
                    ListViewItem lsvitem = new ListViewItem();
                    lsvitem.ImageIndex = 4;
                    lsvitem.Text = fileinfo.Name;
                    listView1.Items.Add(lsvitem);
                    listView1.Items[DirectCount].SubItems.Add(fileinfo.Extension);
                    listView1.Items[DirectCount].SubItems.Add(fileinfo.Length.ToString());
                    listView1.Items[DirectCount].SubItems.Add(fileinfo.FullName);
                   
                    DirectCount++;
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("에러 발생");
            }
            
        }


        //콤보박스를 이용한 ListView 보기 설정
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "Details":
                    listView1.View = View.Details;
                    break;

                case "SmallIcon":
                    listView1.View = View.SmallIcon;
                    break;

                case "LargeIcon":
                    listView1.View = View.LargeIcon;
                    break;

                case "List":
                    listView1.View = View.List;
                    break;

                case "Tile":
                    listView1.View = View.Tile;
                    break;
            }

        }

        //파일 검색
        private void button1_Click(object sender, EventArgs e)
        {
            
            DirectoryInfo d = new DirectoryInfo(textBox2.Text);//주소값을 가져온다
            FileInfo[] file = d.GetFiles(textBox1.Text, SearchOption.AllDirectories);
            int i = 0;
            try
            {
                listView1.Items.Clear();
                foreach(FileInfo a in file)
                {
                    ListViewItem lsvitem = new ListViewItem();
                    lsvitem.ImageIndex = 4; //파일 이미지 인덱스
                    lsvitem.Text = a.Name;
                    listView1.Items.Add(lsvitem); // 이름
                    listView1.Items[i].SubItems.Add(a.Extension); // 확장자
                    listView1.Items[i].SubItems.Add(a.Length.ToString()); // 크기
                    textBox2.Text = listView1.SelectedItems[0].SubItems[3].Text;
                    i++;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("에러 발생");
            }
        }

        //리스트 뷰 더블클릭을 이용한 파일 열기 및 폴더 열기
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems[0].SubItems[1].Text == ".txt")
                System.Diagnostics.Process.Start(listView1.SelectedItems[0].SubItems[3].Text);
            else if (listView1.SelectedItems[0].SubItems[1].Text == ".jpg")
                System.Diagnostics.Process.Start(listView1.SelectedItems[0].SubItems[3].Text);
            else if (listView1.SelectedItems[0].SubItems[1].Text == ".exe")
                System.Diagnostics.Process.Start(listView1.SelectedItems[0].SubItems[3].Text);
            else if (listView1.SelectedItems[0].SubItems[1].Text == ".zip")
                System.Diagnostics.Process.Start(listView1.SelectedItems[0].SubItems[3].Text);
            else
                System.Diagnostics.Process.Start
                    (textBox2.Text+"\\"+listView1.SelectedItems[0].SubItems[0].Text);

        }
        //트리뷰 더블클릭시 그 경로의 파일 오픈
        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(textBox2.Text);
        }

        //엔터 입력 시 검색
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            DirectoryInfo d = new DirectoryInfo(textBox2.Text);//주소값을 가져온다
            FileInfo[] file = d.GetFiles(textBox1.Text, SearchOption.AllDirectories);
            int i = 0;
            try
            {
                listView1.Items.Clear();
                foreach (FileInfo a in file)
                {
                    ListViewItem lsvitem = new ListViewItem();
                    lsvitem.ImageIndex = 4; //파일 이미지 인덱스
                    lsvitem.Text = a.Name;
                    listView1.Items.Add(lsvitem); // 이름
                    listView1.Items[i].SubItems.Add(a.Extension); // 확장자
                    listView1.Items[i].SubItems.Add(a.Length.ToString()); // 크기
                    textBox2.Text = listView1.SelectedItems[0].SubItems[3].Text;
                    i++;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("에러 발생");
            }
        }
    }
}
