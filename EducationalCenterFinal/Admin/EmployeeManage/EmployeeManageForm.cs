﻿using EducationalCenterFinal.Admin.CourseManage;
using EducationalCenterFinal.Admin.CreateAccount;
using EducationalCenterFinal.Admin.Staff.StaffCoursesManage;
using EducationalCenterFinal.Admin.Staff;
using EducationalCenterFinal.Admin.Staff.StudentManage;
using EducationalCenterFinal.Admin.TeacherManage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EducationalCenterFinal.SpecialForms;
using System.Xml.Linq;

namespace EducationalCenterFinal.Admin.EmployeeManage
{
    public partial class EmployeeManageForm : Form
    {
        private List<TextBox> textBoxes = new List<TextBox>();
        readonly private EducationCenterEntities dp = new EducationCenterEntities();
        public EmployeeManageForm()
        {
            InitializeComponent();

            this.ClientSize = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height - 70);
            this.MinimumSize = this.ClientSize;
            this.WindowState = FormWindowState.Maximized;

            LoadEmployeeData();
            ApplyStyling();
            SetupComponents();
            SearchBoxPlaceHolder();
        }

        private void SetupComponents()
        {
            this.questionsToolStripMenuItem.Click += (sender, e) => this.QuestionsToolStripMenuItem_Click("admin");
            this.studentsToolStripMenuItem.Click += (sender, e) => this.StudentsToolStripMenuItem_Click("admin");

            foreach (var course in dp.courses)
            {
                ToolStripMenuItem courseMenuItem = new ToolStripMenuItem
                {
                    Name = $"{course.courseName}ToolStripMenuItem",
                    Size = new System.Drawing.Size(134, 30),
                    Text = course.courseName,
                };
                courseMenuItem.Click += (sender, e) => CourseMenuItem_Click(course.courseId, "admin");
                manageToolStripMenuItem.DropDownItems.Add(courseMenuItem);
            }

            PictureBox pictureBox = new PictureBox
            {
                Image = Image.FromFile(Application.StartupPath.Remove(Application.StartupPath.Length - 10) + "\\Images\\search-interface-symbol.png"),
                SizeMode = PictureBoxSizeMode.Normal,
                Location = new Point(panel1.Width - 30, 13),
                Size = new Size(50, 50)
            };
            panel1.Controls.Add(pictureBox);

            Dictionary<string, Action> buttons = new Dictionary<string, Action>()
            {
                { "Add", AddEmployee },
                { "Edit", EditEmployee },
                { "Reset", ResetFields },
                { "Delete", DeleteEmployee },
            };

            int x = 0, y = 0;
            foreach (var item in buttons)
            {
                Button button = new Button
                {
                    Text = item.Key,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    Location = new Point(x == 0 ? 12 : panel2.Width - 12 - (panel2.Width - 44) / 2, y > 1 ? panel2.Height - 12 - 40 : panel2.Height - 12 - 40 - 20 - 40),
                    Size = new Size((panel2.Width - 44) / 2, 40),
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                };
                button.Click += (sender, e) => item.Value();
                panel2.Controls.Add(button);
                x = x == 20 ? 0 : 20;
                y++;
            }

            List<string> inputs = new List<string> { "U_ID", "Name", "Email", "Role", "Address", "Salary", "Phone" };
            int yOffset = 40, labelY = 12;
            foreach (var placeholderText in inputs)
            {
                Label label = new Label();
                label.ForeColor = Color.White;
                label.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                label.Location = new System.Drawing.Point(9, labelY);
                label.Text = placeholderText;
                Panel panel = new Panel();
                panel.BackColor = Color.FromArgb(236, 236, 236);
                panel.Size = new Size(panel2.Width - 24, 40);
                panel.Padding = new Padding(5, 0, 5, 0);
                panel.Margin = new Padding(0);
                panel.Location = new Point(12, yOffset);
                TextBox input = new TextBox
                {
                    BackColor = Color.FromArgb(236, 236, 236),
                    BorderStyle = BorderStyle.None,
                    Font = new Font("Arial", 12F),
                    ForeColor = Color.Black,
                    Location = new Point(5, 10),
                    Size = new Size(panel.Width - 10, 30),
                    Margin = new Padding(0),
                };
                textBoxes.Add(input);
                panel.Controls.Add(input);
                yOffset += 75;
                labelY += 75;
                panel2.Controls.Add(panel);
                panel2.Controls.Add(label);
            }
        }

        private void LoadEmployeeData(int? filterId = null)
        {
            var employees = dp.staff.Select(s => new {
                ID = s.staffId,
                U_ID = s.userId,
                Name = s.staffName,
                Role = s.role,
                Email = s.staffEmail,
                Phone = s.staffPhone,
                Salary = s.staffSalary,
                Address = s.staffAddress,
            });
            if (filterId.HasValue)
            {
                employees = employees.Where(s => s.ID == filterId.Value);
            }

            dataGridView1.DataSource = employees.ToList();
        }

        private void ApplyStyling()
        {
            panel2.Size = new Size((int)(ClientSize.Width / 3.5) - 24, ClientSize.Height - 130);
            panel2.Location = new Point(ClientSize.Width - 12 - panel2.Width, ClientSize.Height - 12 - panel2.Height);
            dataGridView1.Size = new Size(ClientSize.Width - panel2.Width - 12 - 24, ClientSize.Height - 130);
            dataGridView1.MinimumSize = new Size(ClientSize.Width - panel2.Width - 12 - 24, ClientSize.Height - 130);
            dataGridView1.MaximumSize = new Size(ClientSize.Width - panel2.Width - 12 - 24, ClientSize.Height - 130);
            dataGridView1.Location = new Point(12, ClientSize.Height - 12 - dataGridView1.Height);

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 236, 236);
                column.DefaultCellStyle.SelectionForeColor = Color.Black;
                column.DefaultCellStyle.Font = new Font("Arial", 11F, FontStyle.Regular);
                column.Width = column.HeaderText == "ID" ? 40 : column.HeaderText == "U_ID" ? 60 : ((dataGridView1.Width - dataGridView1.RowHeadersWidth - 40) / 4);
                column.HeaderCell.Style.Alignment = column.HeaderText == "ID" ? DataGridViewContentAlignment.MiddleCenter : column.HeaderText == "U_ID" ? DataGridViewContentAlignment.MiddleCenter : DataGridViewContentAlignment.MiddleLeft;
                column.DefaultCellStyle.Alignment = column.HeaderText == "ID" ? DataGridViewContentAlignment.MiddleCenter : column.HeaderText == "U_ID" ? DataGridViewContentAlignment.MiddleCenter : DataGridViewContentAlignment.MiddleLeft;
            }

            panel1.Width = panel2.Width;
            searchBox.Width = panel2.Width - 40;
            panel1.Location = new Point(ClientSize.Width - 12 - panel1.Width, 50);
            label1.Location = new Point(12, 45);
        }

        private void SearchBoxPlaceHolder()
        {
            searchBox.Text = "Search By ID...";
            searchBox.ForeColor = Color.Gray;
            searchBox.Enter += (sender, e) =>
            {
                if (searchBox.Text == "Search By ID...")
                {
                    searchBox.Text = "";
                    searchBox.ForeColor = Color.Black;
                }
            };
            searchBox.Leave += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(searchBox.Text))
                {
                    searchBox.Text = "Search By ID...";
                    searchBox.ForeColor = Color.Gray;
                }
            };
        }

        private void SearchBox_TextChanged_1(object sender, EventArgs e)
        {
            if (int.TryParse(searchBox.Text, out int id))
            {
                LoadEmployeeData(id);
            }
            else
            {
                LoadEmployeeData();
            }
        }

        private void CourseMenuItem_Click(int CourseId, string role)
        {
            new StaffCourseForm(CourseId, role).Show();
            this.Hide();
        }

        private void CoursesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new CourseManageForm().Show();
            this.Hide();
        }

        private void LogoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LoginForm().Show();
            this.Hide();
        }

        private void ForgetPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ForgotPassword(dp).Show();
        }

        private void CreateAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new CreateAccountForm().Show();
            this.Hide();
        }

        private void TeachersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new TeacherManageForm().Show();
            this.Hide();
        }

        private void QuestionsToolStripMenuItem_Click(string role)
        {
            new QuestionsForm(role).Show();
            this.Hide();
        }

        private void StudentsToolStripMenuItem_Click(string role)
        {
            new StudentManageForm(role).Show();
            this.Hide();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridView1.Rows[e.RowIndex];
                string name = row.Cells["Name"].Value.ToString();
                string U_ID = row.Cells["U_ID"].Value.ToString();
                string email = row.Cells["Email"].Value.ToString();
                string role = row.Cells["Role"].Value.ToString();
                string address = row.Cells["Address"].Value.ToString();
                string salary = row.Cells["Salary"].Value.ToString();
                string phone = row.Cells["Phone"].Value.ToString();
                textBoxes[0].Text = U_ID;
                textBoxes[1].Text = name;
                textBoxes[2].Text = email;
                textBoxes[3].Text = role;
                textBoxes[4].Text = address;
                textBoxes[5].Text = salary;
                textBoxes[6].Text = phone;
            }
        }

        private void AddEmployee()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBoxes[0].Text) ||
                    string.IsNullOrWhiteSpace(textBoxes[1].Text) ||
                    string.IsNullOrWhiteSpace(textBoxes[2].Text) ||
                    string.IsNullOrWhiteSpace(textBoxes[3].Text) ||
                    string.IsNullOrWhiteSpace(textBoxes[4].Text) ||
                    string.IsNullOrWhiteSpace(textBoxes[5].Text) ||
                    string.IsNullOrWhiteSpace(textBoxes[6].Text))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }

                if (!int.TryParse(textBoxes[0].Text.Trim(), out int userID))
                {
                    throw new Exception("Invalid User ID. It must be a number.");
                }

                if (!int.TryParse(textBoxes[5].Text.Trim(), out int staffSalary))
                {
                    throw new Exception("Invalid Salary. It must be an integer.");
                }

                string phoneNumber = textBoxes[6].Text.Trim();
                if (phoneNumber.Length != 11 || !phoneNumber.All(char.IsDigit))
                {
                    throw new Exception("Invalid Phone Number. It must contain exactly 11 digits.");
                }

                string staffEmail = textBoxes[2].Text.Trim();
                var existingEmployee = dp.staff
                    .FirstOrDefault(e => e.userId == userID || e.staffEmail == staffEmail);
                if (existingEmployee != null)
                {
                    throw new Exception("Employee with this User ID or Email already exists.");
                }

                var newEmployee = new staff
                {
                    userId = userID,
                    staffName = textBoxes[1].Text.Trim(),
                    staffEmail = staffEmail,
                    role = textBoxes[3].Text.Trim(),
                    staffAddress = textBoxes[4].Text.Trim(),
                    staffSalary = staffSalary,
                    staffPhone = phoneNumber
                };

                dp.staff.Add(newEmployee);
                dp.SaveChanges();

                MessageBox.Show("Employee added successfully.");
                LoadEmployeeData();
                ResetFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void EditEmployee()
        {
            try
            {
                if (dataGridView1.SelectedRows.Count != 1)
                {
                    MessageBox.Show("Please select one employee to edit.");
                    return;
                }

                var selectedRow = dataGridView1.SelectedRows[0];
                int employeeId = int.Parse(selectedRow.Cells["ID"].Value.ToString());
                var employee = dp.staff.SingleOrDefault(s => s.staffId == employeeId);

                if (employee == null)
                {
                    MessageBox.Show("Employee not found.");
                    return;
                }

                if (employee.userId != int.Parse(textBoxes[0].Text.Trim()))
                {
                    MessageBox.Show("User_ID cannot be edited.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBoxes[1].Text) ||
                    string.IsNullOrWhiteSpace(textBoxes[2].Text) ||
                    string.IsNullOrWhiteSpace(textBoxes[3].Text) ||
                    string.IsNullOrWhiteSpace(textBoxes[4].Text) ||
                    string.IsNullOrWhiteSpace(textBoxes[5].Text) ||
                    string.IsNullOrWhiteSpace(textBoxes[6].Text))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }

                if (!decimal.TryParse(textBoxes[5].Text.Trim(), out decimal staffSalary))
                {
                    MessageBox.Show("Invalid Salary. It must be an integer.");
                    return;
                }

                string phoneNumber = textBoxes[6].Text.Trim();
                if (phoneNumber.Length != 11 || !phoneNumber.All(char.IsDigit))
                {
                    MessageBox.Show("Invalid Phone Number. It must contain exactly 11 digits.");
                    return;
                }
                employee.staffName = textBoxes[1].Text.Trim();
                employee.staffEmail = textBoxes[2].Text.Trim();
                employee.role = textBoxes[3].Text.Trim();
                employee.staffAddress = textBoxes[4].Text.Trim();
                employee.staffSalary = staffSalary;
                employee.staffPhone = phoneNumber;
                dp.SaveChanges();
                MessageBox.Show("Employee updated successfully.");
                LoadEmployeeData();
                ResetFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private void ResetFields()
        {
            foreach (var textBox in textBoxes)
            {
                textBox.Clear();
            }
        }

        private void DeleteEmployee()
        {
            try
            {
                if (dataGridView1.SelectedRows.Count != 1)
                {
                    MessageBox.Show("Please select one employee to delete.");
                    return;
                }
                var selectedRow = dataGridView1.SelectedRows[0];
                int employeeId = int.Parse(selectedRow.Cells["ID"].Value.ToString());
                var employee = dp.staff.SingleOrDefault(s => s.staffId == employeeId);
                if (employee != null)
                {
                    dp.staff.Remove(employee);
                    dp.SaveChanges();
                    MessageBox.Show("Employee deleted successfully.");
                    LoadEmployeeData();
                }
                else
                {
                    MessageBox.Show("Employee not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

    }
}