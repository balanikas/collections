using System;
using System.Collections;
using System.Collections.Generic;

namespace Collections
{
    public static class SupportedTypes
    {
        public static Dictionary<Type, List<string>> Get()
        {
            var collectionTypes = new Dictionary<Type, List<string>>();

            List<string> actionTypes;
            actionTypes = new List<string>();
            actionTypes.Add("Add");
            actionTypes.Add("Clear");
            actionTypes.Add("ToString");
            collectionTypes.Add(typeof (HashSet<>), actionTypes);

            actionTypes = new List<string>();
            actionTypes.Add("Add");
            actionTypes.Add("Clear");
            actionTypes.Add("ToString");
            collectionTypes.Add(typeof (List<>), actionTypes);

            actionTypes = new List<string>();
            actionTypes.Add("Push");
            actionTypes.Add("Clear");
            actionTypes.Add("ToString");
            collectionTypes.Add(typeof (Stack<>), actionTypes);

            actionTypes = new List<string>();
            actionTypes.Add("Add");
            collectionTypes.Add(typeof (Dictionary<,>), actionTypes);

            actionTypes = new List<string>();
            actionTypes.Add("Add");
            collectionTypes.Add(typeof (ArrayList), actionTypes);

            actionTypes = new List<string>();
            actionTypes.Add("Add");
            collectionTypes.Add(typeof (SortedList), actionTypes);


            return collectionTypes;
        }
    }
}