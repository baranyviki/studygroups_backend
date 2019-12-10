namespace StudyGroups.Data.DAL.DAOs
{
    public enum SubjectType
    {
        MathematicalAndNaturalSciences //0
        , EnterpriseInformationSystems //1
        , SoftwareEngineering //2
        , NetworksAndInformationSecurity //3
        , DatabasesAndBigData //4
        , EmbeddedSystems //5
        , Economics //6
        , Humanities //7
        , FoundationSubject //8
        , Other //9 -do not map this
        , ComputerArchitectures //10
    };

    public class Subject
    {
        public string SubjectID { get; set; }
        public string SubjectCode { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public int SuggestedSemester { get; set; }
        public int SubjectType { get; set; }
    }

}
