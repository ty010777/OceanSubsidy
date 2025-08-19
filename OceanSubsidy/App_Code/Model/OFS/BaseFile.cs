using System;

public class BaseFile
{
    public int ID { get; set; }

    public string Name { get; set; }

    public string Path { get; set; }

    public long Size { get; set; }

    public string Type { get; set; }

    public DateTime CreateTime { get; set; }

    public string CreateUser { get; set; }
}
