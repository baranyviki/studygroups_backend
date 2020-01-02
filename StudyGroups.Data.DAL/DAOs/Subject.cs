namespace StudyGroups.Data.DAL.DAOs
{
    public enum SubjectType
    {
        None                                //0
        , MathematicalAndNaturalSciences    //1
        , EnterpriseInformationSystems      //2
        , SoftwareEngineering               //3
        , NetworksAndInformationSecurity    //4
        , DatabasesAndBigData               //5
        , EmbeddedSystems                   //6
        , Economics                         //7
        , Humanities                        //8
        , FoundationSubject                 //9 
        , Other                             //10 -do not map this
        , ComputerArchitectures             //11
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
