using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using DataAccess.SqlServer;
using DataAccess.MySql;
using DataAccess.PostgreSQL;
using DataAccess.SQLite;
using System.Data;

namespace DataAccess.Samples
{
    public static class SampleRunner
    {
        private static IDataStore dstore;

        public static void RunMe()
        {
            bool running = true;
            GetDataStore();

            do
            {
                WriteTitle("Make Selection");
                Console.WriteLine("1: Manage Classes");
                Console.WriteLine("2: Manage Students");
                Console.WriteLine("");
                Console.WriteLine("3: Exit");

                int result = ReadConsoleIntegerInput(3);

                switch (result)
                {
                    case 1:
                        ManageClasses();
                        break;
                    case 2:
                        ManageStudents();
                        break;
                    case 3:
                        running = false;
                        break;
                }
            } while (running);
        }

        private static int ReadConsoleIntegerInput(int max)
        {
            int result = -1;
            do
            {
                Console.Write("Enter Selection: ");
                string read = Console.ReadLine();
                int.TryParse(read, out result);
            } while (result < 0 || result > max);
            return result;
        }

        private static void ManageStudents()
        {
            bool repeat = true;
            do
            {
                WriteTitle("Student Management");
                Console.WriteLine("1: View Students");
                Console.WriteLine("2: Add Student");
                Console.WriteLine("3: Search Students");
                Console.WriteLine("");
                Console.WriteLine("4: Exit");

                switch (ReadConsoleIntegerInput(4))
                {
                    case 1:
                        ViewStudents();
                        break;
                    case 2:
                        AddStudent();
                        break;
                    case 3:
                        SearchStudents();
                        break;
                    case 4:
                        repeat = false;
                        break;
                }
            }
            while (repeat);
        }

        private static void SearchStudents()
        {
            bool repeat = true;
            do
            {
                WriteTitle("Search Students");
                Console.Write("Criteria: ");
                string criteria = Console.ReadLine();
                repeat = WriteStudentMenu(dstore.Query<Student>().Where(r => r.FirstName.Contains(criteria) || r.LastName.Contains(criteria)).ToList(), EditStudent);
            }
            while (repeat);
        }

        private static void AddStudent()
        {
            Student c = new Student();

            WriteTitle("Add Student");
            Console.Write("First Name: ");
            c.FirstName = Console.ReadLine();

            Console.Write("Last Name: ");
            c.LastName = Console.ReadLine();

            dstore.InsertObject(c);
        }

        private static void ViewStudents()
        {
            bool repeat = true;
            do
            {
                WriteTitle("Current Students");
                repeat = WriteStudentMenu(dstore.LoadEntireTable<Student>(), EditStudent);
            } while (repeat);
        }

        private static bool WriteStudentMenu(IEnumerable<Student> students, Action<int> Action)
        {
            if (students.Count() > 0)
            {
                foreach (Student c in students)
                    Console.WriteLine("{0}: {1}, {2}", c.ID, c.LastName, c.FirstName);

                int max = students.Max(r => r.ID);
                Console.WriteLine("");
                Console.WriteLine("{0}: Go Back", max + 1);
                Console.WriteLine("");
                Console.WriteLine("");
                Console.Write("Choose Student: ");
                int choice = ReadConsoleIntegerInput(++max);

                if (choice == max)
                    return false;
                else
                {
                    Action(choice);
                    return true;
                }
            }
            else
            {
                Console.WriteLine("No Students Found");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.Write("Press Enter to Continue");
                Console.ReadLine();
                return false;
            }
        }

        private static void EditStudent(int choice)
        {
            bool repeat = true;

            do
            {
                Student c = dstore.LoadObject<Student>(choice);
                if (c == null) return;

                WriteTitle(string.Format("Manage Student: {0}, {1}", c.FirstName, c.LastName));
                Console.WriteLine("1: Change First Name");
                Console.WriteLine("2: Change Last Name");
                Console.WriteLine("3: Delete");
                Console.WriteLine("4: Assign Class");
                Console.WriteLine("5: View Classes");
                Console.WriteLine("");
                Console.WriteLine("6: Exit");

                switch (ReadConsoleIntegerInput(6))
                {
                    case 1:
                        ChangeFirstName(c);
                        break;
                    case 2:
                        ChangeLastName(c);
                        break;
                    case 3:
                        DeleteStudent(c);
                        break;
                    case 4:
                        AssignClass(c);
                        break;
                    case 5:
                        ViewClasses(c);
                        break;
                    case 6:
                        repeat = false;
                        break;
                }
            }
            while (repeat);
        }

        private static void ViewClasses(Student c)
        { //lets go old school and use a db cmd here
            IDbCommand cmd = dstore.Connection.GetCommand();
            cmd.CommandText = "SELECT cls.* FROM Students std INNER JOIN StudentClasses stdCls on stdCls.Student = std.StudentID INNER JOIN Class cls on cls.ID = stdCls.Class WHERE std.StudentID = @studentID;";

            //small helper method
            cmd.Parameters.Add(dstore.Connection.GetParameter("studentID", c.ID));

            WriteTitle(string.Format("Current Assignments for {0}, {1}", c.LastName, c.FirstName));
            List<Class> classes = dstore.ExecuteCommandLoadList<Class>(cmd).ToList();
            WriteClassMenu(classes, r => ManageStudentClass(r, c.ID));
        }

        private static void AssignClass(Student c)
        {
            WriteTitle("Choose Class");
            WriteClassMenu(dstore.LoadEntireTable<Class>().ToList(), r =>
            {
                StudentClasses stdClass = new StudentClasses();
                stdClass.Student = c.ID;
                stdClass.Class = r;

                dstore.InsertObject(stdClass);
            });
        }

        private static void DeleteStudent(Student c)
        {
            WriteTitle(String.Format("You are about to delete {0}, {1}, this cannot be undone, continue?", c.LastName, c.FirstName));
            Console.WriteLine("1: Yes");
            Console.WriteLine("2: No"); ;

            switch (ReadConsoleIntegerInput(2))
            {
                case 1: //either way works
                    //dstore.DeleteObject(c);
                    dstore.DeleteObject<Student>(c.ID);
                    //dstore.DeleteObject(typeof(Class), c.ID);
                    break;
                case 2:
                    break;
            }
        }

        private static void ChangeLastName(Student c)
        {
            WriteTitle(String.Format("Change Last Name for {0}, {1}", c.LastName, c.FirstName));
            Console.Write("Enter New Name: ");
            c.LastName = Console.ReadLine();
            dstore.UpdateObject(c);
        }

        private static void ChangeFirstName(Student c)
        {
            WriteTitle(String.Format("Change First Name for {0}, {1}", c.LastName, c.FirstName));
            Console.Write("Enter New Name: ");
            c.FirstName = Console.ReadLine();
            dstore.UpdateObject(c);
        }

        private static void ManageClasses()
        {
            bool repeat = true;
            do
            {
                WriteTitle("Class Management");
                Console.WriteLine("1: View Classes");
                Console.WriteLine("2: Add Class");
                Console.WriteLine("3: Search Classes");
                Console.WriteLine("");
                Console.WriteLine("4: Exit");

                switch (ReadConsoleIntegerInput(4))
                {
                    case 1:
                        ViewClasses();
                        break;
                    case 2:
                        AddClass();
                        break;
                    case 3:
                        SearchClasses();
                        break;
                    case 4:
                        repeat = false;
                        break;
                }
            }
            while (repeat);
        }

        private static void SearchClasses()
        {
            bool repeat = true;
            do
            {
                WriteTitle("Search Classes");
                Console.Write("Criteria: ");
                repeat = WriteClassMenu(dstore.Query<Class>().Where(r => r.Name.Contains(Console.ReadLine())).ToList(), ManageClass);
            }
            while (repeat);
        }

        private static void AddClass()
        {
            WriteTitle("Add Class");
            Console.Write("New Class Name: ");

            Class c = new Class();
            c.Name = Console.ReadLine();

            dstore.InsertObject(c);
        }

        private static void WriteTitle(string title)
        {
            Console.Clear();
            Console.WriteLine("-------- " + title + " --------");
        }

        private static void ViewClasses()
        {
            bool repeat = true;
            do
            {
                WriteTitle("Current Classes");
                repeat = WriteClassMenu(dstore.LoadEntireTable<Class>().ToList(), ManageClass);
            } while (repeat);
        }

        private static bool WriteClassMenu(IList<Class> classes, Action<int> MangeFunction)
        {
            if (classes.Count() > 0)
            {
                foreach (Class c in classes)
                    Console.WriteLine("{0}: {1}", c.ID, c.Name);

                int max = classes.Max(r => r.ID);
                Console.WriteLine("");
                Console.WriteLine("{0}: Go Back", max + 1);
                Console.WriteLine("");
                Console.WriteLine("");
                Console.Write("Choose Class: ");
                int choice = ReadConsoleIntegerInput(++max);

                if (choice == max)
                    return false;
                else
                {
                    MangeFunction(choice);
                    return true;
                }
            }
            else
            {
                Console.WriteLine("No Classes Found");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.Write("Press Enter to Continue");
                Console.ReadLine();
                return false;
            }
        }

        private static void ManageStudentClass(int Class, int Student)
        {
            bool repeat = true;

            do
            {
                Class c = (Class)dstore.LoadObject(typeof(Class), Class); //lets load a different way here

                if (c == null) return;

                WriteTitle(string.Format("Manage Student Class Assignment: {0}", c.Name));
                Console.WriteLine("1: Remove Assignment");
                Console.WriteLine("");
                Console.WriteLine("2: Exit");

                switch (ReadConsoleIntegerInput(2))
                {
                    case 1:
                        RemoveClassAssignment(c, dstore.LoadObject<Student>(Student));
                        break;
                    case 2:
                        repeat = false;
                        break;
                }
            }
            while (repeat);
        }

        private static void RemoveClassAssignment(Class c, Student std)
        {
            WriteTitle("You are about to delete the following Class Assignment");
            WriteTitle(string.Format("{0}, {1} -> {2} this cannot be undone, continue?", c.Name, std.LastName, std.FirstName));
            Console.WriteLine("1: Yes");
            Console.WriteLine("2: No"); ;

            switch (ReadConsoleIntegerInput(2))
            {
                case 1:
                    dstore.DeleteObject(new StudentClasses()
                    {
                        Class = c.ID,
                        Student = std.ID
                    });
                    //dstore.DeleteObject(typeof(Class), c.ID); // this method wont work here since this object has more than 1 key
                    break;
                case 2:
                    break;
            }
        }

        private static void ManageClass(int choice)
        {
            bool repeat = true;

            do
            {
                Class c = dstore.LoadObject<Class>(choice);
                if (c == null) return;

                WriteTitle(string.Format("Manage Class: {0}", c.Name));
                Console.WriteLine("1: Change Name");
                Console.WriteLine("2: Delete");
                Console.WriteLine("3: Assign Students");
                Console.WriteLine("4: View Students");
                Console.WriteLine("");
                Console.WriteLine("5: Exit");

                switch (ReadConsoleIntegerInput(5))
                {
                    case 1:
                        RenameClass(c);
                        break;
                    case 2:
                        Delete(c);
                        break;
                    case 3:
                        AssignStudents(c);
                        break;
                    case 4:
                        ViewStudents(c);
                        break;
                    case 5:
                        repeat = false;
                        break;
                }
            }
            while (repeat);
        }

        private static void ViewStudents(Class c)
        {//lets use linq this time
            List<Student> students = (from i in dstore.Query<Student>()
                                      join x in dstore.Query<StudentClasses>() on i.ID equals x.Student
                                      join z in dstore.Query<Class>() on x.Class equals z.ID
                                      where z.ID == c.ID
                                      select i).ToList();

            WriteTitle(string.Format("Current Students for {0}", c.Name));
            WriteStudentMenu(students, r => ManageStudentClass(c.ID, r));
        }

        private static void AssignStudents(Class c)
        {
            WriteTitle("Assign Student to " + c.Name);
            WriteStudentMenu(dstore.LoadEntireTable<Student>(), r =>
            {
                StudentClasses stdClas = new StudentClasses();
                stdClas.Class = c.ID;
                stdClas.Student = r;
                dstore.InsertObject(stdClas);
            });
        }

        private static void Delete(Class c)
        {
            WriteTitle(String.Format("You are about to delete {0}, this cannot be undone, continue?", c.Name));
            Console.WriteLine("1: Yes");
            Console.WriteLine("2: No"); ;

            switch (ReadConsoleIntegerInput(2))
            {
                case 1: //either way works
                    dstore.DeleteObject(c);
                    //dstore.DeleteObject<Class>(c.ID);
                    //dstore.DeleteObject(typeof(Class), c.ID);
                    break;
                case 2:
                    break;
            }
        }

        private static void RenameClass(Class c)
        {
            WriteTitle(String.Format("Rename {0}", c.Name));
            Console.Write("Enter New Name: ");
            c.Name = Console.ReadLine();
            dstore.UpdateObject(c);
        }

        private static void GetDataStore()
        {
            dstore = SqlServerConnection.GetDataStore("devserver", "DataAccess", "AppLogin", "AppLogin");
            //dstore = MySqlServerConnection.GetDataStore("devserver", "DataAccess", "AppLogin", "AppLogin");
            //dstore = PostgreSQLServerConnection.GetDataStore("devserver", "DataAccess", "AppLogin", "AppLogin");
            //dstore = SqlLiteDataConnection.GetDataStore("Examples.db");
        }
    }
}
