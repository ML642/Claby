

using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
namespace Graphs;


public class PathFinderVisitor : IVertexVisitor
{
    public int target;
    public List<int> Path{get;}

    bool found = false;
    public bool Found{get=>found;}
    public PathFinderVisitor(int target)
    {
        this.target = target;
        this.Path = new List<int>();
    }

    public void PreVisit(int vertex)
    {   
        if (found == false){
            Path.Add(vertex);
        }
        if(vertex == target)
        {
            found = true;
        }
        
    }
    public void PostVisit(int vertex)
        {if (found == false){
                
                Path.Remove(vertex);
        }
        
    }
    public override string ToString()
    {   
        if(found == false){
            return "NOT FOUND \n";
        }
        StringBuilder ds = new StringBuilder();

        foreach(int i in Path)
        {
            if(i!=target){
            ds.Append(i);
            ds.Append("-->");
            }
            else
            {
                ds.Append(i);
            }

        }
        return ds.ToString();
    }
};



public interface IVertexVisitor
{
    public void PreVisit(int vertex);
    public void PostVisit(int vertex);
}

public abstract class Graph
{
    protected int vertexCount{get;init;}

    public int VertexCount{get=>vertexCount;}
    protected Graph(int vertexCount)
    {
        this.vertexCount = vertexCount;
    }

    public  abstract List<int> GetOutVertices(int vertex);
    public abstract void AddEdge(int from,int to);

    public void Display()
    {   
        for(int i = 0 ; i < this.vertexCount; i++)
        {
            List<int> ver = GetOutVertices(i);
            Console.Write(i);
            Console.Write(" -> [");
            
            int count = 1;
            foreach(int j in ver){
                if(count != ver.Count()){
                count++;
                Console.Write(j);
                Console.Write(',');
                }
                else
                {
                    Console.Write(j);
                }
            }
            Console.Write("]\n");
        }
    }
    public  abstract void DepthFirstSearch(int start, IVertexVisitor visitor);

    
}


public class AdjacencyListGraph:Graph
{
    
    List<int>[] AdjacencyGraph;
    public AdjacencyListGraph(int vertexCount):base(vertexCount)
    {
        this.AdjacencyGraph = new List<int>[vertexCount];
        for(int i = 0 ; i < vertexCount ; i++)
        {
            AdjacencyGraph[i] = new List<int>(); 
        }
    }

    public override List<int> GetOutVertices(int vertex)
    {
        List<int> allNodes = new List<int>();

        foreach(int i in AdjacencyGraph[vertex]){
             allNodes.Add(i);   
        }
        return allNodes;
    }
    public override void AddEdge(int from,int to)
    {
        AdjacencyGraph[from].Add(to);
    }

    private void DepthFirstSearch1(int start, IVertexVisitor visitor,bool[] visited)
    {
        List<int> neighbors = GetOutVertices(start);
        foreach(int i in neighbors)
        {
           
            if(visited[i] == false){
                 visitor.PreVisit(i);
                 visited[i] = true;
                 DepthFirstSearch1(i,visitor,visited);
                 visitor.PostVisit(i);
            }
            
        }
    }
    public  override void DepthFirstSearch(int start, IVertexVisitor visitor)
    {
        bool[] visited = new bool[vertexCount];

            visited[start] = true;
            visitor.PreVisit(start);
            DepthFirstSearch1(start, visitor, visited);
            visitor.PostVisit(start);
    }
}

public class AdjacencyMatrixGraph:Graph

{
    bool[,] boolean;
    public AdjacencyMatrixGraph(int vertexCount):base(vertexCount)
    {
        this.boolean = new bool[vertexCount,vertexCount];  
    }

    public override List<int> GetOutVertices(int vertex)
    {
        List<int> allNodes = new List<int>();

        for(int i = 0 ; i < vertexCount ; i++)
        {
            if (boolean[vertex, i])
            {
                allNodes.Add(i);
            }
        }
        return allNodes;
    }
    public override void AddEdge(int from,int to)
    {
        boolean[from,to] = true;
    }
    private void DepthFirstSearch1(int start, IVertexVisitor visitor,bool[] visited)
    {
        List<int> neighbors = GetOutVertices(start);
        
        foreach(int i in neighbors)
        {
            
           
            if(visited[i] == false){
                 visited[i] = true;
                 visitor.PreVisit(i);
                 DepthFirstSearch1(i,visitor,visited); 
                 visitor.PostVisit(i);
            }
           
           
        }
    }
    public  override void DepthFirstSearch(int start, IVertexVisitor visitor)
    {
        bool[] value  = new bool[vertexCount];
       
            value[start] = true;
            visitor.PreVisit(start);
            DepthFirstSearch1(start, visitor, value);
            visitor.PostVisit(start);
    }
}

