using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace huaanClient
{
    class recursionJSON
    {
        public class TreeModel11
        {
            public string id { set; get; }
            public string name { set; get; }
            public string phone { set; get; }
            public string no { set; get; }
            public string ParentId { set; get; }
            public string address { set; get; }
            public string explain { set; get; }
            public string code { set; get; }
            public string publish_time { set; get; }
            public string title { set; get; }
        }

        /// <summary>
        /// 构建树形结构类
        /// </summary>
        public class TreeModel
        {
            public string id { set; get; }
            public string name { set; get; }
            public string phone { set; get; }
            public string no { set; get; }
            public string ParentId { set; get; }
            public string address { set; get; }
            public string explain { set; get; }
            public string code { set; get; }
            public string publish_time { set; get; }
            public string title { set; get; }
            public List<TreeModel> children { set; get; }
        }

        /// <summary>
        /// 公用递归(反射转换List)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="allList">数据列表</param>
        /// <param name="parentId">父级ID</param>
        /// <param name="idField">id字段名称</param>
        /// <param name="parentIdField">父级id字段名称</param>
        /// <param name="nameField">name字段名称</param>
        /// <returns></returns>
        public static List<TreeModel> ConversionList<T>(List<T> allList, string parentId, string idField, string parentIdField, string name, string phone, string no, string address, string explain, string code,string title)
        {
            List<TreeModel> list = new List<TreeModel>();
            TreeModel model = null;
            foreach (var item in allList)
            {
                model = new TreeModel();
                foreach (System.Reflection.PropertyInfo p in item.GetType().GetProperties())
                {
                    if (p.Name == idField)
                    {
                        model.id = p.GetValue(item,null).ToString();
                    }
                    if (p.Name == parentIdField)
                    {
                        model.ParentId = p.GetValue(item, null).ToString();
                    }
                    if (p.Name == name)
                    {
                        model.name = p.GetValue(item, null).ToString();
                    }
                    if (p.Name == phone)
                    {
                        model.phone = p.GetValue(item, null)?.ToString();
                    }
                    if (p.Name == no)
                    {
                        model.no = p.GetValue(item, null).ToString();
                    }
                    if (p.Name == address)
                    {
                        model.address = p.GetValue(item, null)?.ToString();
                    }
                    if (p.Name == explain)
                    {
                        model.explain = p.GetValue(item, null)?.ToString();
                    }
                    if (p.Name == code)
                    {
                        model.code = p.GetValue(item, null).ToString();
                    }
                    if (p.Name == title)
                    {
                        model.title = p.GetValue(item, null).ToString();
                    }
                }
                list.Add(model);
            }
            return OperationParentData(list, parentId);
        }

        /// <summary>
        /// 公用递归(处理递归最父级数据)
        /// </summary>
        /// <param name="treeDataList">树形列表数据</param>
        /// <param name="parentId">父级Id</param>
        /// <returns></returns>
        public static List<TreeModel> OperationParentData(List<TreeModel> treeDataList, string parentId)
        {
            var data = treeDataList.Where(x => x.ParentId == parentId);
            List<TreeModel> list = new List<TreeModel>();
            foreach (var item in data)
            {
                OperationChildData(treeDataList, item);
                list.Add(item);
            }
            return list;
        }
        /// <summary>
        /// 公用递归(递归子级数据)
        /// </summary>
        /// <param name="treeDataList">树形列表数据</param>
        /// <param name="parentItem">父级model</param>
        public static  void OperationChildData(List<TreeModel> treeDataList, TreeModel parentItem)
        {
            var subItems = treeDataList.Where(ee => ee.ParentId == parentItem.id).ToList();
            if (subItems.Count != 0)
            {
                parentItem.children = new List<TreeModel>();
                parentItem.children.AddRange(subItems);
                foreach (var subItem in subItems)
                {
                    OperationChildData(treeDataList, subItem);
                }
            }
        }
    }
}
