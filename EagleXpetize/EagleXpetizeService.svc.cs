using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;
using System.Drawing.Imaging;
using System.Net;

namespace EagleXpetize
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class EagleXpetizeService : IEagleXpetizeService
    {
        string constr = ConfigurationSettings.AppSettings["ConnectionString"];
        string path = System.AppDomain.CurrentDomain.BaseDirectory + @"\NImage\";
        string filepath;
        public User CheckUserLogin(string userName, string password, string userType)
        {
            return GetUserByCredentials(userName, password, userType);
        }

        protected User GetUserByCredentials(string userName, string password, string type)
        {
            var user = new User();

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("spc_User_CheckLogins", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@uname", SqlDbType.VarChar, 200).Value = userName;
                cmd.Parameters.Add("@passwrd", SqlDbType.VarChar, 200).Value = password;
                cmd.Parameters.Add("@type", SqlDbType.VarChar, 200).Value = type;

                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                con.Close();
            }

            if (dt.Rows.Count != 0)
            {
                DataRow row = dt.Rows[0];
                if (row["UserId"] != null)
                    user.UserId = Convert.ToInt32(row["UserId"].ToString());
                if (row["UserName"] != null)
                    user.UserName = row["UserName"].ToString();
                if (row["Password"] != null)
                    user.Password = row["Password"].ToString();
                user.Type = type;
                user.Message = "Success";
            }
            else
            {
                user.Message = "Failed";
            }

            var jsonStr = new JavaScriptSerializer().Serialize(user);

            return user; //jsonStr;
        }

        public User GetUserDetails(string UserId)
        {
            User data = new User();

            data.UserId = 1;
            data.UserName = "Vijay";
            data.Password = "12345";
            data.Type = "Manager";

            return data;
        }

        public List<User> UsersListByType(string userType)
        {
            return GetUsersByType(userType);
        }

        public List<Notification> Notifications(string id, string isSubTask)
        {
            return GetNotification(id, isSubTask);
        }

        public List<CheckList> CheckList(string id)
        {
            return GetCheckList(id);
        }

        public List<Attachment> Attachment(string id)
        {
            return GetAttachment(id);
        }

        public List<Task> Tasks(string id, string name, string status)
        {
            if (name.Trim() == "0")
                name = "";

            //TaskDetails m = new TaskDetails();
            //m.TaskDetailsId = 0;
            //m.TaskId = 0;
            //m.StatusId = 0;
            //m.AssignedById = 0;
            //m.AssignedToId = 1;
            //m.IsSubTask = true;

            //var tt = TaskAssigned(m);

            //TaskDetails mUpd = new TaskDetails();
            //mUpd.TaskDetailsId = 3;
            //mUpd.TaskId = 1;
            //mUpd.AssignedById = 3;
            //mUpd.AssignedToId = 1;
            //mUpd.StatusId = 3;
            //mUpd.IsSubTask = true;
            //mUpd.Comments = "testing the update";
            //mUpd.CreatedBy = "3";
            //mUpd.StartDateStr = "2016-07-01 11:46:40.417";
            ////mUpd.StartDate = Convert.ToDateTime(mUpd.StartDateStr);
            //mUpd.EndDateStr = "2016-07-06 11:46:40.418";
            ////mUpd.EndDate = Convert.ToDateTime(mUpd.EndDateStr);

            //UpdateAssignedTask(mUpd);

            return GetTaskWithSubTask(Convert.ToInt32(id), Convert.ToInt32(status), name);
        }

        public List<SubTask> SubTasks(string id, string task, string name, string status, string priority)
        {
            if (name.Trim() == "0")
                name = "";
            return GetSubTaskList(Convert.ToInt32(id), Convert.ToInt32(task), name, Convert.ToInt32(status), Convert.ToInt32(priority));
        }

        protected List<User> GetUsersByType(string type)
        {
            var userList = new List<User>();

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("spc_User_GetByType", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@type", SqlDbType.VarChar, 200).Value = type;

                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                con.Close();
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var user = new User();
                    if (row["UserId"] != null)
                        user.UserId = Convert.ToInt32(row["UserId"].ToString());
                    if (row["UserName"] != null)
                        user.UserName = row["UserName"].ToString();
                    if (row["Password"] != null)
                        user.Password = row["Password"].ToString();
                    if (row["TypeId"] != null)
                        user.TypeId = Convert.ToInt32((row["TypeId"]));
                    if (row["IsActive"] != null)
                        user.IsActive = (bool)(row["IsActive"]);
                    user.Type = type;

                    userList.Add(user);
                }
            }

            var jsonStr = new JavaScriptSerializer().Serialize(userList);

            return userList; //jsonStr;
        }

        protected List<Task> GetTaskWithSubTask(int id, int status, string name)
        {
            var taskList = new List<Task>();

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("[dbo].[spc_Task_Get]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@status", SqlDbType.Int).Value = status;
                cmd.Parameters.Add("@name", SqlDbType.NVarChar, 200).Value = name;

                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                con.Close();
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var task = new Task();

                    if (row["TaskId"] != null)
                        task.TaskId = (int)(row["TaskId"]);
                    if (row["TaskName"] != null)
                        task.TaskName = row["TaskName"].ToString();
                    if (row["Description"] != null)
                        task.Description = row["Description"].ToString();
                    if (row["Comments"] != null)
                        task.Comments = row["Comments"].ToString();
                    if (row["IsActive"] != null)
                        task.IsActive = (bool)(row["IsActive"]);
                    if (row["CreatedDate"] != null)
                    {
                        task.CreatedDate = (DateTime)(row["CreatedDate"]);
                        task.CreatedDateStr = row["CreatedDate"].ToString();
                    }
                    if (row["CreatedBy"] != null)
                        task.CreatedBy = row["CreatedBy"].ToString();
                    if (row["ModifiedBy"] != null)
                        task.ModifiedBy = row["ModifiedBy"].ToString();
                    if (row["ModifiedDate"] != null)
                    {
                        task.ModifiedDate = (DateTime)(row["ModifiedDate"]);
                        task.ModifiedDateStr = row["ModifiedDate"].ToString();
                    }
                    if (row["StatusId"] != null)
                        task.StatusId = (int)(row["StatusId"]);
                    if (row["Status"] != null)
                        task.Status = row["Status"].ToString();
                    if (row["Location"] != null)
                        task.Location = row["Location"].ToString();
                    if (row["TaskOrder"] != null && row["TaskOrder"].ToString() != "")
                        task.TaskOrder = (int)(row["TaskOrder"]);

                    task.SubTasks = new List<SubTask>();
                    task.SubTasks = GetSubTaskList(0, task.TaskId, "", 0, 0);

                    taskList.Add(task);
                }
            }


            var jsonStr = new JavaScriptSerializer().Serialize(taskList);

            return taskList; //jsonStr;
        }

        protected List<SubTask> GetSubTaskList(int id, int taskid, string name, int status, int priority)
        {
            var subTaskList = new List<SubTask>();

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("[dbo].[spc_SubTask_Get]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@task", SqlDbType.Int).Value = taskid;
                cmd.Parameters.Add("@status", SqlDbType.Int).Value = status;
                cmd.Parameters.Add("@priority", SqlDbType.Int).Value = priority;
                cmd.Parameters.Add("@name", SqlDbType.NVarChar, 200).Value = name;

                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                con.Close();
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var subTask = new SubTask();

                    if (row["SubTaskId"] != null)
                        subTask.SubTaskId = Convert.ToInt32(row["SubTaskId"].ToString());
                    if (row["TaskId"] != null)
                        subTask.TaskId = (int)(row["TaskId"]);
                    if (row["SubTaskName"] != null)
                        subTask.SubTaskName = row["SubTaskName"].ToString();
                    if (row["Description"] != null)
                        subTask.Description = row["Description"].ToString();
                    if (row["Comments"] != null)
                        subTask.Comments = row["Comments"].ToString();
                    if (row["IsActive"] != null)
                        subTask.IsActive = (bool)(row["IsActive"]);
                    if (row["CreatedDate"] != null)
                    {
                        subTask.CreatedDate = (DateTime)(row["CreatedDate"]);
                        subTask.CreatedDateStr = row["CreatedDate"].ToString();
                    }
                    if (row["CreatedBy"] != null)
                        subTask.CreatedBy = row["CreatedBy"].ToString();
                    if (row["ModifiedBy"] != null)
                        subTask.ModifiedBy = row["ModifiedBy"].ToString();
                    if (row["ModifiedDate"] != null)
                    {
                        subTask.ModifiedDate = (DateTime)(row["ModifiedDate"]);
                        subTask.ModifiedDateStr = row["ModifiedDate"].ToString();
                    }
                    if (row["StatusId"] != null)
                        subTask.StatusId = (int)(row["StatusId"]);
                    if (row["Status"] != null)
                        subTask.Status = row["Status"].ToString();
                    if (row["PriorityId"] != null)
                        subTask.PriorityId = (int)(row["PriorityId"]);
                    if (row["Priority"] != null)
                        subTask.Priority = row["Priority"].ToString();
                    if (row["TaskOrder"] != null && row["TaskOrder"].ToString() != "")
                        subTask.TaskOrder = (int)(row["TaskOrder"]);

                    subTaskList.Add(subTask);
                }
            }


            var jsonStr = new JavaScriptSerializer().Serialize(subTaskList);

            return subTaskList; //jsonStr;
        }

        public List<CommanMasters> CommanMasterList(string id, string source)
        {
            var lstMasters = GetCommanMasterDetails(Convert.ToInt32((id)), source).ToList();
            return lstMasters;
        }

        protected List<CommanMasters> GetCommanMasterDetails(int id, string source)
        {
            var masters = new List<CommanMasters>();

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string spName = "";

                if (source == "status")
                    spName = "[dbo].[spc_TaskStatus_Get]";
                else if (source == "priority")
                    spName = "[dbo].[spc_Priority_Get]";

                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                con.Close();
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var master = new CommanMasters();

                    if (row["Id"] != null)
                        master.Id = (int)(row["Id"]);
                    if (row["Name"] != null)
                        master.Name = row["Name"].ToString();
                    if (row["IsActive"] != null)
                        master.IsActive = (bool)(row["IsActive"]);
                    if (row["CreatedDate"] != null && row["CreatedDate"].ToString() != "")
                    {
                        master.CreatedDate = (DateTime)(row["CreatedDate"]);
                        master.CreatedDateStr = row["CreatedDate"].ToString();
                    }
                    if (row["CreatedBy"] != null)
                        master.CreatedBy = row["CreatedBy"].ToString();
                    if (row["ModifiedBy"] != null)
                        master.ModifiedBy = row["ModifiedBy"].ToString();
                    if (row["ModifiedDate"] != null && row["ModifiedDate"].ToString() != "")
                    {
                        master.ModifiedDate = (DateTime)(row["ModifiedDate"]);
                        master.ModifiedDateStr = row["ModifiedDate"].ToString();
                    }

                    masters.Add(master);
                }
            }


            var jsonStr = new JavaScriptSerializer().Serialize(masters);

            return masters; //jsonStr;
        }

        public string NewUser(User user)
        {
            return user.UserName;
        }

        public string NewTask(Task task)
        {
            return AddNewTask(task);
        }

        public string NewNotification(Notification notification)
        {
            return AddNotification(notification);
        }

        public string NewHistory(History history)
        {
            return AddHistory(history);
        }

        public string NewCheckList(CheckList checklist)
        {
            return AddCheckList(checklist);
        }

        public string NewAttachment(Attachment attachment)
        {
            return AddTaskAttachment(attachment);
        }

        public string NewCheckListAttachment(CheckListAttachment listattachment)
        {
            return AddCheckListAttachment(listattachment);
        }

        public string UpdateCheckListAttachment(CheckListAttachment listattachment)
        {
            return UpdCheckListAttachment(listattachment);
        }

        //public string NewCheckListAttach(CheckListAttachment checklistAttach)
        //{
        //    return AddCheckListAttachment(checklistAttach);
        //}

        public string NewSubTask(SubTask subTask)
        {
            if (!string.IsNullOrEmpty(subTask.CreatedDateStr))
                subTask.CreatedDate = Convert.ToDateTime(subTask.CreatedDateStr);
            else
                subTask.CreatedDate = null;
            if (!string.IsNullOrEmpty(subTask.ModifiedDateStr))
                subTask.ModifiedDate = Convert.ToDateTime(subTask.ModifiedDateStr);
            else
                subTask.ModifiedDate = null;

            return AddNewSubTask(subTask);
        }

        public string AssignTask(TaskDetails taskDetails)
        {
            if (!string.IsNullOrEmpty(taskDetails.AssignedDateStr))
                taskDetails.AssignedDate = Convert.ToDateTime(taskDetails.AssignedDateStr);
            else
                taskDetails.AssignedDate = null;

            return AssignTaskToUser(taskDetails);
        }

        public string AddTokenNew(TokenDtl TkDtl)
        {
            //if (!string.IsNullOrEmpty(taskDetails.AssignedDateStr))
            //    taskDetails.AssignedDate = Convert.ToDateTime(taskDetails.AssignedDateStr);
            //else
            //    taskDetails.AssignedDate = null;

            return AddFcmTokenNew(TkDtl);
        }

        public string AddToken(User user)
        {
            return AddFcmToken(user);
        }

        public string UpdateTask(Task task)
        {
            return UpdateTask(task);
        }

        public string UpdateSubTask(SubTask subTask)
        {
            return UpdateSubTask(subTask);
        }

        public string UpdateAssignedTask(TaskDetails taskDetails)
        {
            if (!string.IsNullOrEmpty(taskDetails.StartDateStr))
                taskDetails.StartDate = Convert.ToDateTime(taskDetails.StartDateStr);
            else
                taskDetails.StartDate = null;

            if (!string.IsNullOrEmpty(taskDetails.EndDateStr))
                taskDetails.EndDate = Convert.ToDateTime(taskDetails.EndDateStr);
            else
                taskDetails.EndDate = null;

            if (!string.IsNullOrEmpty(taskDetails.ModifiedDateStr))
                taskDetails.ModifiedDate = Convert.ToDateTime(taskDetails.ModifiedDateStr);
            else
                taskDetails.ModifiedDate = null;

            return UpdateExistingTaskDetails(taskDetails);
        }

        public List<TaskDetails> TaskAssigned(TaskDetails taskDetails)
        {
            return GetTaskDetails(taskDetails);
        }

        protected string AddNewTask(Task m)
        {
            string msg = "success";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("[dbo].[spc_Task_Add]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar, 500).Value = m.TaskName;
                    cmd.Parameters.Add("@desc", SqlDbType.VarChar).Value = m.Description;
                    cmd.Parameters.Add("@place", SqlDbType.VarChar, 200).Value = m.Location;
                    cmd.Parameters.Add("@comments", SqlDbType.VarChar).Value = m.Comments;
                    cmd.Parameters.Add("@status", SqlDbType.Int).Value = m.StatusId;
                    cmd.Parameters.Add("@taskOrder", SqlDbType.Int).Value = m.TaskOrder;
                    cmd.Parameters.Add("@userId", SqlDbType.VarChar, 200).Value = m.CreatedBy;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return msg;
        }

        protected string AddNewSubTask(SubTask m)
        {
            string msg = "success";

            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("[dbo].[spc_SubTask_Add]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@task", SqlDbType.Int).Value = m.TaskId;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar, 500).Value = m.SubTaskName;
                    cmd.Parameters.Add("@desc", SqlDbType.VarChar).Value = m.Description;
                    cmd.Parameters.Add("@createDt", SqlDbType.DateTime).Value = m.CreatedDate;
                    cmd.Parameters.Add("@modifyDt", SqlDbType.DateTime).Value = m.ModifiedDate;
                    cmd.Parameters.Add("@taskOrder", SqlDbType.Int).Value = m.TaskOrder;
                    cmd.Parameters.Add("@status", SqlDbType.Int).Value = m.StatusId;
                    cmd.Parameters.Add("@priority", SqlDbType.Int).Value = m.PriorityId;
                    cmd.Parameters.Add("@comments", SqlDbType.VarChar).Value = m.Comments;
                    cmd.Parameters.Add("@userId", SqlDbType.VarChar, 200).Value = m.CreatedBy;

                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    con.Close();
                }

                if (dt.Rows.Count != 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["SubTaskId"] != null)
                            msg = row["SubTaskId"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return msg;
        }

        protected string AssignTaskToUser(TaskDetails m)
        {
            string msg = "success";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("[dbo].[spc_TaskDetails_Add]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@task", SqlDbType.Int).Value = m.TaskId;
                    cmd.Parameters.Add("@assignedTo", SqlDbType.Int).Value = m.AssignedToId;
                    cmd.Parameters.Add("@assignedBy", SqlDbType.Int).Value = m.AssignedById;
                    cmd.Parameters.Add("@startDate", SqlDbType.DateTime).Value = m.StartDate;
                    cmd.Parameters.Add("@endDate", SqlDbType.DateTime).Value = m.EndDate;
                    cmd.Parameters.Add("@status", SqlDbType.Int).Value = m.StatusId;
                    cmd.Parameters.Add("@comments", SqlDbType.VarChar).Value = m.Comments;
                    cmd.Parameters.Add("@isSubTask", SqlDbType.Bit).Value = m.IsSubTask;
                    cmd.Parameters.Add("@userId", SqlDbType.VarChar, 200).Value = m.CreatedBy;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return msg;
        }

        protected string AddFcmTokenNew(TokenDtl m)
        {
            string msg = "success";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("[dbo].[spc_token_Add]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@userId", SqlDbType.Int).Value = m.UserId;
                    cmd.Parameters.Add("@token", SqlDbType.NVarChar).Value = m.Token;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return msg;
        }

        protected string UpdateExistingTask(Task m)
        {
            string msg = "success";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("[dbo].[spc_Task_Upd]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = m.TaskId;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar, 500).Value = m.TaskName;
                    cmd.Parameters.Add("@desc", SqlDbType.VarChar).Value = m.Description;
                    cmd.Parameters.Add("@place", SqlDbType.VarChar, 200).Value = m.Location;
                    cmd.Parameters.Add("@comments", SqlDbType.VarChar).Value = m.Comments;
                    cmd.Parameters.Add("@status", SqlDbType.Int).Value = m.StatusId;
                    cmd.Parameters.Add("@taskOrder", SqlDbType.Int).Value = m.TaskOrder;
                    cmd.Parameters.Add("@userId", SqlDbType.VarChar, 200).Value = m.CreatedBy;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return msg;
        }

        protected string UpdateExistingSubTask(SubTask m)
        {
            string msg = "success";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("[dbo].[spc_SubTask_Upd]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = m.SubTaskId;
                    cmd.Parameters.Add("@task", SqlDbType.Int).Value = m.TaskId;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar, 500).Value = m.SubTaskName;
                    cmd.Parameters.Add("@desc", SqlDbType.VarChar).Value = m.Description;
                    cmd.Parameters.Add("@modifiedDate", SqlDbType.Int).Value = m.ModifiedDate;
                    cmd.Parameters.Add("@taskOrder", SqlDbType.Int).Value = m.TaskOrder;
                    cmd.Parameters.Add("@status", SqlDbType.Int).Value = m.StatusId;
                    cmd.Parameters.Add("@priority", SqlDbType.Int).Value = m.PriorityId;
                    cmd.Parameters.Add("@comments", SqlDbType.VarChar).Value = m.Comments;
                    cmd.Parameters.Add("@userId", SqlDbType.VarChar, 200).Value = m.CreatedBy;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return msg;
        }

        protected string UpdateExistingTaskDetails(TaskDetails m)
        {
            string msg = "success";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("[dbo].[spc_TaskDetails_Upd]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = m.TaskDetailsId;
                    cmd.Parameters.Add("@task", SqlDbType.Int).Value = m.TaskId;
                    cmd.Parameters.Add("@assignedTo", SqlDbType.Int).Value = m.AssignedToId;
                    cmd.Parameters.Add("@assignedBy", SqlDbType.Int).Value = m.AssignedById;
                    cmd.Parameters.Add("@startDate", SqlDbType.DateTime).Value = m.StartDate;
                    cmd.Parameters.Add("@endDate", SqlDbType.DateTime).Value = m.EndDate;
                    cmd.Parameters.Add("@status", SqlDbType.Int).Value = m.StatusId;
                    cmd.Parameters.Add("@comments", SqlDbType.VarChar).Value = m.Comments;
                    cmd.Parameters.Add("@isSubTask", SqlDbType.Bit).Value = m.IsSubTask;
                    cmd.Parameters.Add("@userId", SqlDbType.VarChar, 200).Value = m.CreatedBy;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return msg;
        }

        protected string AddFcmToken(User m)
        {
            string msg = "success";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("[dbo].[spc_token_Add]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@userId", SqlDbType.Int).Value = m.UserId;
                    cmd.Parameters.Add("@token", SqlDbType.NVarChar, 500).Value = m.token;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return msg;
        }

        protected List<TaskDetails> GetTaskDetails(TaskDetails m)
        {
            var assignedTaskList = new List<TaskDetails>();

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("[dbo].[spc_TaskDetails_Get]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = m.TaskDetailsId;
                cmd.Parameters.Add("@task", SqlDbType.Int).Value = m.TaskId;
                cmd.Parameters.Add("@assignedTo", SqlDbType.Int).Value = m.AssignedToId;
                cmd.Parameters.Add("@assignedBy", SqlDbType.NVarChar, 200).Value = m.AssignedById;
                cmd.Parameters.Add("@isSubTask", SqlDbType.NVarChar, 200).Value = m.IsSubTask;
                cmd.Parameters.Add("@status", SqlDbType.Int).Value = m.StatusId;
                cmd.Parameters.Add("@assignedDate", SqlDbType.DateTime).Value = m.AssignedDate;

                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                con.Close();
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var task = new TaskDetails();

                    if (row["TaskDetailsId"] != null)
                        task.TaskDetailsId = Convert.ToInt32(row["TaskDetailsId"].ToString());
                    if (row["TaskId"] != null)
                        task.TaskId = (int)(row["TaskId"]);
                    if (row["TaskName"] != null)
                        task.TaskName = row["TaskName"].ToString();
                    if (row["TaskDescription"] != null)
                        task.TaskDescription = row["TaskDescription"].ToString();
                    if (row["AssignedToId"] != null)
                        task.AssignedToId = Convert.ToInt32(row["AssignedToId"].ToString());
                    if (row["AssignedById"] != null)
                        task.AssignedById = Convert.ToInt32(row["AssignedById"].ToString());
                    if (row["AssignedByName"] != null)
                        task.AssignedByName = row["AssignedByName"].ToString();
                    if (row["AssignedDate"] != null && row["AssignedDate"].ToString() != "")
                    {
                        task.AssignedDate = Convert.ToDateTime(row["AssignedDate"].ToString());
                        task.AssignedDateStr = row["AssignedDate"].ToString();
                    }
                    if (row["StartDate"] != null && row["StartDate"].ToString() != "")
                    {
                        task.StartDate = Convert.ToDateTime(row["StartDate"].ToString());
                        task.StartDateStr = row["StartDate"].ToString();
                    }
                    if (row["EndDate"] != null && row["EndDate"].ToString() != "")
                    {
                        task.EndDate = Convert.ToDateTime(row["EndDate"].ToString());
                        task.EndDateStr = row["EndDate"].ToString();
                    }
                    if (row["Comments"] != null)
                        task.Comments = row["Comments"].ToString();
                    if (row["IsActive"] != null)
                        task.IsActive = (bool)(row["IsActive"]);
                    if (row["CreatedDate"] != null)
                    {
                        task.CreatedDate = (DateTime)(row["CreatedDate"]);
                        task.CreatedDateStr = row["CreatedDate"].ToString();
                    }
                    if (row["CreatedBy"] != null)
                        task.CreatedBy = row["CreatedBy"].ToString();
                    if (row["ModifiedBy"] != null)
                        task.ModifiedBy = row["ModifiedBy"].ToString();
                    if (row["ModifiedDate"] != null)
                    {
                        task.ModifiedDate = (DateTime)(row["ModifiedDate"]);
                        task.ModifiedDateStr = row["ModifiedDate"].ToString();
                    }
                    if (row["StatusId"] != null)
                        task.StatusId = (int)(row["StatusId"]);
                    if (row["Status"] != null)
                        task.Status = row["Status"].ToString();
                    if (row["TaskOrder"] != null && row["TaskOrder"].ToString() != "")
                        task.TaskOrder = (int)(row["TaskOrder"]);

                    assignedTaskList.Add(task);
                }
            }


            var jsonStr = new JavaScriptSerializer().Serialize(assignedTaskList);

            return assignedTaskList.OrderBy(x => x.TaskOrder).ToList(); //jsonStr;
        }

        protected List<Notification> GetNotification(string id, string isSubTask)
        {
            var notificationList = new List<Notification>();

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("[dbo].[spc_Notification_Get]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@isSubTask", SqlDbType.Int).Value = isSubTask;
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                con.Close();
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var notification = new Notification();

                    if (row["TaskId"] != null)
                        notification.TaskId = Convert.ToInt32(row["TaskId"].ToString());
                    if (row["Description"] != null)
                        notification.Description = row["Description"].ToString();
                    if (row["ToId"] != null)
                        notification.ToId = Convert.ToInt32(row["ToId"].ToString());
                    if (row["ById"] != null)
                        notification.ById = Convert.ToInt32(row["ById"].ToString());
                    if (row["IsNew"] != null)
                        notification.IsNew = (bool)(row["IsNew"]);
                    if (row["UserName"] != null)
                        notification.UserName = row["UserName"].ToString();
                    if (row["TaskName"] != null)
                        notification.TaskName = row["TaskName"].ToString();
                    notificationList.Add(notification);
                }
            }

            var jsonStr = new JavaScriptSerializer().Serialize(notificationList);

            return notificationList; //jsonStr;

        }

        protected void GetToken(int id, string message)
        {
            var notificationList = new List<Notification>();

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("[dbo].[spc_token_Get]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                con.Close();
                string Token;
                Token = dt.Rows[0][1].ToString();
                FcmPush(Token, message);
            }
        }

        protected List<CheckList> GetCheckList(string id)
        {
            var checkListData = new List<CheckList>();

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("[dbo].[spc_CheckList_Get]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                con.Close();
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var checklist = new CheckList();

                    if (row["Id"] != null)
                        checklist.Id = Convert.ToInt32(row["Id"].ToString());
                    if (row["TaskId"] != null)
                        checklist.TaskId = Convert.ToInt32(row["TaskId"].ToString());
                    if (row["ItemList"] != null)
                        checklist.ItemListString = row["ItemList"].ToString();
                    if (row["Checked"] != null)
                        checklist.Checked = (bool)row["Checked"];
                    if (row["IsSubTask"] != null)
                        checklist.IsSubTask = (bool)row["IsSubTask"];
                    if (row["CreatedBy"] != null)
                        checklist.CreatedBy = Convert.ToInt32(row["CreatedBy"].ToString());
                    if (row["ModifiedBy"] != null)
                        checklist.ModifiedBy = Convert.ToInt32(row["ModifiedBy"].ToString());
                    if (row["StatusId"] != null)
                        checklist.StatusId = Convert.ToInt32(row["StatusId"].ToString());
                    checkListData.Add(checklist);
                }
            }

            var jsonStr = new JavaScriptSerializer().Serialize(checkListData);

            return checkListData; //jsonStr;

        }

        protected string AddNotification(Notification m)
        {
            string msg = "success";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("[dbo].[spc_Notification_Add]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@text", SqlDbType.NVarChar, 500).Value = m.Description;
                    cmd.Parameters.Add("@taskId", SqlDbType.VarChar).Value = m.TaskId;
                    cmd.Parameters.Add("@notificationBy", SqlDbType.VarChar, 200).Value = m.ById;
                    cmd.Parameters.Add("@notificationTo", SqlDbType.VarChar).Value = m.ToId;
                    cmd.Parameters.Add("@createdBy", SqlDbType.Bit).Value = m.CreatedBy;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    GetToken(m.ToId, m.Description);
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return msg;
        }

        protected string AddHistory(History m)
        {
            string msg = "success";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("[dbo].[spc_History_Add]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@taskId", SqlDbType.Int).Value = m.TaskId;
                    cmd.Parameters.Add("@isSubTask", SqlDbType.Bit).Value = m.IsSubTask;
                    cmd.Parameters.Add("@notes", SqlDbType.VarChar).Value = m.Notes;
                    cmd.Parameters.Add("@comments", SqlDbType.VarChar).Value = m.Comments;
                    cmd.Parameters.Add("@historyDate", SqlDbType.Int).Value = m.HistoryDate;
                    cmd.Parameters.Add("@createdDate", SqlDbType.Int).Value = m.CreatedDate;
                    cmd.Parameters.Add("@createdBy", SqlDbType.VarChar).Value = m.CreatedBy;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return msg;
        }

        protected string AddCheckList(CheckList m)
        {
            string msg = "success";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    int cnt = 0;
                    foreach (string str in m.ItemList)
                    {
                        if (cnt == 0)
                        {
                            SqlCommand cmdDel = new SqlCommand("[dbo].[spc_CheckList_Delete]", con);
                            cmdDel.CommandType = CommandType.StoredProcedure;
                            cmdDel.Parameters.Add("@taskId", SqlDbType.Int).Value = m.TaskId;
                            con.Open();
                            cmdDel.ExecuteNonQuery();
                            con.Close();
                        }

                        SqlCommand cmd = new SqlCommand("[dbo].[spc_CheckList_Add]", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@taskId", SqlDbType.Int).Value = m.TaskId;
                        cmd.Parameters.Add("@isSubTask", SqlDbType.Bit).Value = m.IsSubTask;
                        cmd.Parameters.Add("@ItemList", SqlDbType.VarChar).Value = str;
                        cmd.Parameters.Add("@Checked", SqlDbType.Bit).Value = m.Checked;
                        cmd.Parameters.Add("@status", SqlDbType.Int).Value = m.StatusId;
                        cmd.Parameters.Add("@modifiedBy", SqlDbType.Int).Value = m.ModifiedBy;
                        cmd.Parameters.Add("@createdBy", SqlDbType.Int).Value = m.CreatedBy;

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        cnt = cnt + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return msg;
        }

        protected string AddCheckListAttachment(CheckListAttachment m)
        {

            string msg = "Success";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    filepath = path + "CheckList" + m.Id + ".jpg";
                    SqlCommand cmd = new SqlCommand("[dbo].[spc_CheckListAttachment_Add]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = m.Id;
                    cmd.Parameters.Add("@FilePath", SqlDbType.VarChar).Value = filepath;
                    cmd.Parameters.Add("@createdBy", SqlDbType.Int).Value = m.CreatedBy;
                    cmd.Parameters.Add("@status", SqlDbType.Int).Value = m.StatusId;
                    cmd.Parameters.Add("@modifiedBy", SqlDbType.Int).Value = m.ModifiedBy;

                    var bytes = Convert.FromBase64String(m.File);
                    using (var imageFile = new FileStream(filepath, FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                Library.WriteErrorLog(msg);
            }
            return msg;

        }

        protected string UpdCheckListAttachment(CheckListAttachment m)
        {

            string msg = "Success";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    //filepath = path + "CheckList" + m.Id + ".jpg";
                    SqlCommand cmd = new SqlCommand("[dbo].[spc_status_Upd]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = m.Id;
                    cmd.Parameters.Add("@createdBy", SqlDbType.Int).Value = m.CreatedBy;
                    cmd.Parameters.Add("@Comments", SqlDbType.VarChar).Value = m.Comments;
                    cmd.Parameters.Add("@status", SqlDbType.Int).Value = m.StatusId;
                    cmd.Parameters.Add("@modifiedBy", SqlDbType.Int).Value = m.ModifiedBy;

                    //var bytes = Convert.FromBase64String(m.File);
                    //using (var imageFile = new FileStream(filepath, FileMode.Create))
                    //{
                    //    imageFile.Write(bytes, 0, bytes.Length);
                    //    imageFile.Flush();
                    //}

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                Library.WriteErrorLog(msg);
            }
            return msg;

        }

        protected string AddTaskAttachment(Attachment m)
        {

            string msg = "success";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    filepath = path + "SubTask" + m.TaskId + "." + m.FileType;
                    SqlCommand cmd = new SqlCommand("[dbo].[spc_TaskAttachment_Add]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@taskId", SqlDbType.Int).Value = m.TaskId;
                    cmd.Parameters.Add("@isSubTask", SqlDbType.Bit).Value = m.IsSubTask;
                    cmd.Parameters.Add("@FilePath", SqlDbType.VarChar).Value = filepath;
                    cmd.Parameters.Add("@FileType", SqlDbType.VarChar).Value = m.FileType;
                    cmd.Parameters.Add("@createdBy", SqlDbType.VarChar).Value = m.CreatedBy;
                    cmd.Parameters.Add("@modifiedBy", SqlDbType.VarChar).Value = m.CreatedBy;
                    if (m.FileType == "mp3")
                    {
                        byte[] bytes = Convert.FromBase64String(m.File);
                        // instance a memory stream and pass the
                        // byte array to its constructor
                        MemoryStream ms = new MemoryStream(bytes);

                        // instance a filestream pointing to the 
                        // storage folder, use the original file name
                        // to name the resulting file
                        FileStream fs = new FileStream
                            (//System.Web.Hosting.HostingEnvironment.MapPath("~/TransientStorage/") +
                            filepath, FileMode.Create);

                        // write the memory stream containing the original
                        // file as a byte array to the filestream
                        ms.WriteTo(fs);

                        // clean up
                        ms.Close();
                        fs.Close();
                        fs.Dispose();
                    }
                    else
                    {

                        var bytes = Convert.FromBase64String(m.File);
                        using (var imageFile = new FileStream(filepath, FileMode.Create))
                        {
                            imageFile.Write(bytes, 0, bytes.Length);
                            imageFile.Flush();
                        }
                    }
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                Library.WriteErrorLog(msg);
            }

            return msg;
        }

        protected List<Attachment> GetAttachment(string id)
        {
            var attachmentList = new List<Attachment>();

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("[dbo].[spc_TaskAttachment_Get]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                con.Close();
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var attachment = new Attachment();

                    if (row["TaskId"] != null)
                        attachment.TaskId = Convert.ToInt32(row["TaskId"].ToString());
                    if (row["IsSubTask"] != null)
                        attachment.IsSubTask = row["IsSubTask"].ToString();
                    if (row["ImageEncodedStr"] != null)
                        attachment.File = row["ToId"].ToString();
                    attachmentList.Add(attachment);
                }
            }

            var jsonStr = new JavaScriptSerializer().Serialize(attachmentList);

            return attachmentList; //jsonStr;

        }

        protected void FcmPush(string token, string message)
        {
            try
            {
                var applicationID = "AIzaSyAA09zkWLqIRoo7rpnGzvnliXmXUZ9rbEg";
                var senderId = "193637113157";
                string deviceId = token;
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    notification = new
                    {
                        body = message
                        //title = "This is the title",
                        //icon = "myicon"
                    },
                    priority = "high"
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);

                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                //Response.Write(sResponseFromServer);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                //Response.Write(ex.Message);
            }

        }

    }
}
