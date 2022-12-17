using System.Text;

namespace Demo;

public class Hehe : AbstractDemo, IDemo
{
    public ObsoleteAttribute? DoSomething()
    {
        // linq test case
        var list = new List<int> { 1, 2, 3, 4, 5 };
        a = 1;
        IEnumerable<int> c = from i in list
                             where i > 2
                             select i;
        
        var rs = list.Where(x => x > 3).ToList();
        return null;
    }

    public override void Run()
    {
        string a = "abc";
        a.Concat("def");
        Console.WriteLine(a);
    }

    public override void Test()
    {
        throw new NotImplementedException();
    }
}