﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BookmarksManager
    {
    public class BookmarkFolder : Collection<IBookmarkItem>, IBookmarkFolder
        {

        /// <summary>
        /// Bookmark title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Time when bookmark was edited in browser
        /// </summary>
        public DateTime? LastModified { get; set; }

        /// <summary>
        /// Time when bookmark was added to browser
        /// </summary>
        public DateTime? Added { get; set; }

        private IDictionary<string, string> _attributes;
        public IDictionary<string, string> Attributes
            {
            get => _attributes ?? (_attributes = new Dictionary<string, string>());
            set => _attributes = value;
            }

        /// <summary>
        /// Path to the bookmark in the browser
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// All links from all folders in flat structure
        /// </summary>
        public IEnumerable<BookmarkLink> AllLinks => this.GetAllItems<BookmarkLink>();

        /// <summary>
        /// All items (links, folders and custom IBookmarkItem objects) in flat structure
        /// </summary>
        public IEnumerable<IBookmarkItem> AllItems => this.GetAllItems<IBookmarkItem>();

        /// <summary>
        /// All folders in flat structure
        /// </summary>
        public IEnumerable<IBookmarkFolder> AllFolders => this.GetAllItems<IBookmarkFolder>();

        public BookmarkFolder()
            {

            }

        public BookmarkFolder(string title) : this()
            {
            Title = title;
            }

        /// <summary>
        /// Returns all items of <typeparamref name="T"/> in flat structure 
        /// </summary>
        /// <typeparam name="T">Specifies what type of items to return</typeparam>
        /// <returns>Flattened list of <typeparamref name="T"/> items</returns>
        public virtual IEnumerable<T> GetAllItems<T>() where T : class, IBookmarkItem
            {
            return this.GetAllItems<T>(this);
            }


        private IEnumerable<T> GetAllItems<T>(IBookmarkFolder folder) where T : class, IBookmarkItem
            {
            foreach (var item in folder)
                {
                if (item is T returnItem)
                    {
                    yield return returnItem;
                    }

                if (item is IBookmarkFolder innerFolder)
                    {
                    foreach (var innerItem in GetAllItems<T>(innerFolder))
                        {
                        yield return innerItem;
                        }
                    }
                }
            }

        public override string ToString()
            {
            return $">>> {Title} <<<";
            }

        /// <summary>
        /// Recursively sets path for all folders and links
        /// </summary>
        /// <param name="parentPath"></param>
        public void SetFolderPaths(string parentPath = "")
            {
            SetFolderPaths(this, parentPath);
            }

        /// <summary>
        /// Recursively sets path for all folders and links in the folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="parentPath"></param>
        public void SetFolderPaths(BookmarkFolder folder, string parentPath = "")
            {
            folder.Path = string.IsNullOrEmpty(parentPath) ? folder.Title : $"{parentPath}/{folder.Title}";

            foreach (var item in folder)
                {
                if (item is BookmarkFolder subFolder)
                    {
                    SetFolderPaths(subFolder, folder.Path);
                    }
                else if (item is BookmarkLink link)
                    {
                    link.Path = folder.Path ?? "/"; // if path is null, set it to root
                    }
                }
            }
        }
    }