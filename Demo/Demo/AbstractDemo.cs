namespace Demo;

public abstract class AbstractDemo: IDemo2
{
     public AbstractDemo()
     {
          Console.WriteLine("AbstractDemo Constructor");
     }
     public abstract void Run();
     protected int a;
     protected int b;
     public void sleep() 
     {
          Console.WriteLine("Zzz");
     }
}

public abstract class AbstractDemo2: AbstractDemo
{
     public abstract void Run2();

}