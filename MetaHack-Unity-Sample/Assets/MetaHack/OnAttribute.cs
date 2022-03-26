
[System.AttributeUsage(System.AttributeTargets.Method)]
public class OnAttribute : System.Attribute {  
    string _name;
    public OnAttribute(string name) {
        _name = name;
    }  
}