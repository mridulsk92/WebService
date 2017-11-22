using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace EagleXpetize
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IEagleXpetizeService
    {
        // TODO: Add your service operations here
        [OperationContract]
        [WebGet(UriTemplate = "CheckUserLogin/{userName}/{password}/{userType}", ResponseFormat = WebMessageFormat.Json)]
        User CheckUserLogin(string userName, string password, string userType);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetUserDetails/{UserId}")]
        User GetUserDetails(string UserId);

        [OperationContract]
        [WebGet(UriTemplate = "UsersListByType/{userType}", ResponseFormat = WebMessageFormat.Json)]
        List<User> UsersListByType(string userType);

        [OperationContract]
        [WebGet(UriTemplate = "Notifications/{id}/{isSubTask}", ResponseFormat = WebMessageFormat.Json)]
        List<Notification> Notifications(string id, string isSubTask);

        [OperationContract]
        [WebGet(UriTemplate = "CheckLists/{id}", ResponseFormat = WebMessageFormat.Json)]
        List<CheckList> CheckList(string id);

        [OperationContract]
        [WebGet(UriTemplate = "Attachments/{id}", ResponseFormat = WebMessageFormat.Json)]
        List<Attachment> Attachment(string id);

        [OperationContract]
        [WebGet(UriTemplate = "Tasks/{id}/{name}/{status}", ResponseFormat = WebMessageFormat.Json)]
        List<Task> Tasks(string id, string name, string status);

        [OperationContract]
        [WebGet(UriTemplate = "SubTasks/{id}/{task}/{name}/{status}/{priority}", ResponseFormat = WebMessageFormat.Json)]
        List<SubTask> SubTasks(string id, string task,string name, string status, string priority);

        [OperationContract]
        [WebGet(UriTemplate = "CommanMasterList/{id}/{source}", ResponseFormat = WebMessageFormat.Json)]
        List<CommanMasters> CommanMasterList(string id, string source);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "NewUser", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string NewUser(User user);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "NewTask", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string NewTask(Task task);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "NewNotification", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string NewNotification(Notification notification);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "NewHistory", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string NewHistory(History history);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "NewCheckList", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string NewCheckList(CheckList checklist);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "NewAttachment", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string NewAttachment(Attachment attachment);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "NewCheckListAttachment", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string NewCheckListAttachment(CheckListAttachment listattachment);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateCheckList", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string UpdateCheckListAttachment(CheckListAttachment listattachment);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "NewSubTask", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string NewSubTask(SubTask subTask);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "AssignTask", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string AssignTask(TaskDetails taskDetails);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "AddTokenNew", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string AddTokenNew(TokenDtl TkDtl);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "AddToken", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string AddToken(User user);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateTask", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string UpdateTask(Task task);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateSubTask", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string UpdateSubTask(SubTask subTask);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateAssignedTask", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string UpdateAssignedTask(TaskDetails taskDetails);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "TaskAssigned", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<TaskDetails> TaskAssigned(TaskDetails taskDetails);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    [Serializable()]
    public class User
    {
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public int TypeId { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public bool token { get; set; }
    }

    [DataContract]
    [Serializable()]
    public class Notification
    {
        [DataMember]
        public int TaskId { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int ById { get; set; }
        [DataMember]
        public int ToId { get; set; }
        [DataMember]
        public int CreatedBy { get; set; }
        [DataMember]
        public Boolean IsNew { get; set; }
        [DataMember]
        public String UserName { get; set; }
        [DataMember]
        public String TaskName { get; set; }
    }


    [DataContract]
    [Serializable()]
    public class History
    {
        [DataMember]
        public int TaskId { get; set; }
        [DataMember]
        public string Notes { get; set; }
        [DataMember]
        public string Comments { get; set; }
        [DataMember]
        public DateTime? HistoryDate { get; set; }
        [DataMember]
        public int CreatedBy { get; set; }
        [DataMember]
        public Boolean IsSubTask { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public string CreatedDateStr { get; set; }
        [DataMember]
        public string HistoryDateStr { get; set; }
    }

    [DataContract]
    [Serializable()]
    public class CheckList
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int TaskId { get; set; }
        [DataMember]
        public int CreatedBy { get; set; }
        [DataMember]
        public bool IsSubTask { get; set; }
        [DataMember]
        public bool Checked { get; set; }
        [DataMember]
        public string [] ItemList { get; set; }
        [DataMember]
        public string ItemListString { get; set; }
        [DataMember]
        public int ModifiedBy { get; set; }
        [DataMember]
        public int StatusId { get; set; }

    }

    [DataContract]
    [Serializable()]
    public class Attachment
    {
        [DataMember]
        public int TaskId { get; set; }
        [DataMember]
        public string IsSubTask { get; set; }
        [DataMember]
        public string File { get; set; }
        [DataMember]       
        public string FileType { get; set; }
        [DataMember]
        public int CreatedBy { get; set; }
        [DataMember]
        public Boolean ModifiedBy { get; set; }
    }

    [DataContract]
    [Serializable()]
    public class CheckListAttachment
    {

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int TaskId { get; set; }    
        [DataMember]
        public string File { get; set; }       
        [DataMember]
        public int CreatedBy { get; set; }
        [DataMember]
        public int ModifiedBy { get; set; }
        [DataMember]
        public int StatusId { get; set; }
        [DataMember]
        public string Comments { get; set; }
    }


    [DataContract]
    [Serializable()]
    public class Task
    {
        [DataMember]
        public int TaskId { get; set; }
        [DataMember]
        public string TaskName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Location { get; set; }
        [DataMember]
        public int StatusId { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public DateTime? ModifiedDate { get; set; }
        [DataMember]
        public string CreatedDateStr { get; set; }
        [DataMember]
        public string ModifiedDateStr { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public List<SubTask> SubTasks { get; set; }
        [DataMember]
        public string Comments { get; set; }
        [DataMember]
        public int? TaskOrder { get; set; }

    }

    [DataContract]
    [Serializable()]
    public class SubTask
    {
        [DataMember]
        public int SubTaskId { get; set; }
        [DataMember]
        public int TaskId { get; set; }
        [DataMember]
        public string SubTaskName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int StatusId { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public int PriorityId { get; set; }
        [DataMember]
        public string Priority { get; set; }
        [DataMember]
        public int? TaskOrder { get; set; }
        [DataMember]
        public string Comments { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public DateTime? ModifiedDate { get; set; }
        [DataMember]
        public string CreatedDateStr { get; set; }
        [DataMember]
        public string ModifiedDateStr { get; set; }
        [DataMember]
        public bool IsActive { get; set; }

    }

    [DataContract]
    [Serializable()]
    public class TaskDetails
    {
        [DataMember]
        public int TaskDetailsId { get; set; }
        [DataMember]
        public int TaskId { get; set; }
        [DataMember]
        public string TaskName { get; set; }
        [DataMember]
        public string TaskDescription { get; set; }
        [DataMember]
        public int AssignedToId { get; set; }
        [DataMember]
        public string AssignedToName { get; set; }
        [DataMember]
        public int AssignedById { get; set; }
        [DataMember]
        public string AssignedByName { get; set; }
        [DataMember]
        public DateTime? AssignedDate { get; set; }
        [DataMember]
        public string AssignedDateStr { get; set; }
        [DataMember]
        public DateTime? StartDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public string StartDateStr { get; set; }
        [DataMember]
        public string EndDateStr { get; set; }

        [DataMember]
        public int StatusId { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Comments { get; set; }
        [DataMember]
        public bool IsSubTask { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public DateTime? ModifiedDate { get; set; }
        [DataMember]
        public string CreatedDateStr { get; set; }
        [DataMember]
        public string ModifiedDateStr { get; set; }
        [DataMember]
        public int? TaskOrder { get; set; }
    }

    [DataContract]
    [Serializable()]
    public class TokenDtl
    {
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public string Token { get; set; }
        
    }


    [DataContract]
    [Serializable()]
    public class CommanMasters
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public DateTime? ModifiedDate { get; set; }
        [DataMember]
        public string CreatedDateStr { get; set; }
        [DataMember]
        public string ModifiedDateStr { get; set; }
    }

}
