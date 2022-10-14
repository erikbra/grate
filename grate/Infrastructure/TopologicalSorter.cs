using System;
using System.Collections.Generic;
using grate.Migration;
using Microsoft.Extensions.Logging;

namespace grate.Infrastructure;

internal class TopologicalSorter
{
    private class Dependency
    {
        internal string Name { get; }
        internal string[] Dependencies { get; }

        internal Dependency(string name, string[] dependencies)
        {
            Name = name;
            Dependencies = dependencies;
        }
    }

    private readonly int[] _vertices;
    private readonly int[,] _matrix;
    private int _verticesCount;
    private readonly int[] _sortedArray;
    private readonly List<Dependency> _dependencies;
    private readonly ILogger<GrateMigrator> _logger;


    public TopologicalSorter(int size, ILogger<GrateMigrator> logger)
    {
        _logger = logger;
        _vertices = new int[size];
        _matrix = new int[size, size];
        _verticesCount = 0;
        for (var i = 0; i < size; i++)
            for (var j = 0; j < size; j++)
                _matrix[i, j] = 0;
        _sortedArray = new int[size];
        _dependencies = new List<Dependency>(size);
    }

    public void AddToList(string name, string[] dependencies)
    {
        _dependencies.Add(new Dependency(name, dependencies));
    }

    public int[] Sort()
    {

        var indexes = new Dictionary<string, int>();

        for (var i = 0; i < _dependencies.Count; i++)
        {
            indexes[_dependencies[i].Name.ToLower()] = AddVertex(i);
        }

        for (var i = 0; i < _dependencies.Count; i++)
        {
            if (_dependencies[i].Dependencies == null) continue;
            foreach (var t in _dependencies[i].Dependencies)
            {
                AddEdge(i, indexes[t.ToLower()]);
            }


        }
        for (var i = 0; i < _dependencies.Count; i++)
        {
            if (_matrix[i, i] != 1)
                continue;

            _logger.Log(LogLevel.Error, "{Name} has a cyclic dependency loop on itself", _dependencies[i].Name);
            throw new ApplicationException($"{_dependencies[i].Name} has a cyclic dependency loop on itself!");
        }

        var result = SortData();
        return result;
    }

    private int AddVertex(int vertex)
    {
        _vertices[_verticesCount++] = vertex;
        return _verticesCount - 1;
    }

    private void AddEdge(int start, int end)
    {
        _matrix[start, end] = 1;
    }

    private int[] SortData()
    {
        while (_verticesCount > 0)
        {
            var currentVertex = NoSuccessors();

            _sortedArray[_verticesCount - 1] = _vertices[currentVertex];

            DeleteVertex(currentVertex);
        }

        return _sortedArray;

        int NoSuccessors()
        {
            var lr = 0;
            var lc = 0;
            for (var row = 0; row < _verticesCount; row++)
            {
                var isEdge = false;
                lr = row;
                for (var col = 0; col < _verticesCount; col++)
                {
                    lc = col;
                    if (_matrix[row, col] <= 0) continue;
                    isEdge = true;
                    break;
                }
                if (!isEdge)
                    return row;
            }
            _logger.Log(LogLevel.Error, "Files {Name} & {Name} share cyclic dependency loop!", _dependencies[lr].Name, _dependencies[lc].Name);
            throw new ApplicationException($"Files {_dependencies[lr].Name} & {_dependencies[lc].Name} share cyclic dependency loop!");
        }
    }

    private void DeleteVertex(int delVert)
    {
        if (delVert != _verticesCount - 1)
        {
            for (var j = delVert; j < _verticesCount - 1; j++)
                _vertices[j] = _vertices[j + 1];

            for (var row = delVert; row < _verticesCount - 1; row++)
                MoveRowUp(row, _verticesCount);

            for (var col = delVert; col < _verticesCount - 1; col++)
                MoveColLeft(col, _verticesCount - 1);
        }
        _verticesCount--;

        void MoveRowUp(int row, int length)
        {
            for (var col = 0; col < length; col++)
                _matrix[row, col] = _matrix[row + 1, col];
        }

        void MoveColLeft(int col, int length)
        {
            for (var row = 0; row < length; row++)
                _matrix[row, col] = _matrix[row, col + 1];
        }
    }
}
