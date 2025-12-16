/**
 * Wiki Models
 * TypeScript interfaces and types for wiki functionality
 */

/**
 * Represents a wiki article
 */
export interface WikiArticle {
  id: number;
  title: string;
  slug: string;
  content: string; // Markdown content
  categoryId: number;
  categoryName?: string;
  authorId: string;
  authorName?: string;
  createdDate: Date;
  updatedDate: Date;
  publishedDate?: Date;
  status: WikiArticleStatus;
  version: number;
  viewCount: number;
  tags: string[];
  isLocked: boolean;
  lockedBy?: string;
  lockedUntil?: Date;
}

/**
 * Wiki article status
 */
export enum WikiArticleStatus {
  Draft = 'draft',
  Published = 'published',
  Archived = 'archived'
}

/**
 * Represents a wiki category
 */
export interface WikiCategory {
  id: number;
  name: string;
  slug: string;
  description?: string;
  parentId?: number;
  articleCount: number;
  order: number;
  icon?: string;
  color?: string;
}

/**
 * Represents a wiki article version in the history
 */
export interface WikiArticleVersion {
  id: number;
  articleId: number;
  version: number;
  title: string;
  content: string;
  authorId: string;
  authorName: string;
  createdDate: Date;
  changesSummary?: string;
  diffStats?: {
    added: number;
    removed: number;
    changed: number;
  };
}

/**
 * Represents a table of contents entry
 */
export interface TocEntry {
  id: string;
  level: number; // 1-6 for h1-h6
  title: string;
  anchor: string;
  children?: TocEntry[];
}

/**
 * Represents a search result
 */
export interface WikiSearchResult {
  article: WikiArticle;
  relevance: number; // 0-100
  matchedContent: string; // Snippet of matched content
  highlights: WikiSearchHighlight[];
}

/**
 * Represents a highlighted portion in search results
 */
export interface WikiSearchHighlight {
  text: string;
  startIndex: number;
  endIndex: number;
}

/**
 * Search filters for wiki articles
 */
export interface WikiSearchFilters {
  query: string;
  categoryId?: number;
  status?: WikiArticleStatus;
  tags?: string[];
  authorId?: string;
  dateFrom?: Date;
  dateTo?: Date;
}

/**
 * Sort options for wiki articles
 */
export interface WikiSortOption {
  value: string;
  label: string;
}

/**
 * Available wiki sort options
 */
export const WIKI_SORT_OPTIONS: WikiSortOption[] = [
  { value: 'title-asc', label: 'Title (A-Z)' },
  { value: 'title-desc', label: 'Title (Z-A)' },
  { value: 'updated-desc', label: 'Recently Updated' },
  { value: 'updated-asc', label: 'Least Recently Updated' },
  { value: 'created-desc', label: 'Newest First' },
  { value: 'created-asc', label: 'Oldest First' },
  { value: 'views-desc', label: 'Most Viewed' },
  { value: 'views-asc', label: 'Least Viewed' }
];

/**
 * Represents active editors for collaborative editing
 */
export interface ActiveEditor {
  userId: string;
  userName: string;
  userAvatar?: string;
  cursorPosition?: number;
  selectedText?: {
    start: number;
    end: number;
  };
  color: string; // Editor's assigned color
  lastActivity: Date;
}

/**
 * Represents a change made by a collaborative editor
 */
export interface CollaborativeChange {
  userId: string;
  timestamp: Date;
  changeType: 'insert' | 'delete' | 'replace';
  position: number;
  content: string;
}

/**
 * Markdown editor toolbar button configuration
 */
export interface MarkdownToolbarButton {
  id: string;
  label: string;
  icon: string;
  action: MarkdownToolbarAction;
  shortcut?: string;
}

/**
 * Markdown toolbar actions
 */
export enum MarkdownToolbarAction {
  Bold = 'bold',
  Italic = 'italic',
  Strikethrough = 'strikethrough',
  Heading1 = 'heading1',
  Heading2 = 'heading2',
  Heading3 = 'heading3',
  Link = 'link',
  Image = 'image',
  Code = 'code',
  CodeBlock = 'codeblock',
  Quote = 'quote',
  UnorderedList = 'unorderedlist',
  OrderedList = 'orderedlist',
  Table = 'table',
  HorizontalRule = 'horizontalrule',
  Undo = 'undo',
  Redo = 'redo',
  Preview = 'preview',
  Fullscreen = 'fullscreen'
}

/**
 * Markdown toolbar button groups
 */
export const MARKDOWN_TOOLBAR_BUTTONS: MarkdownToolbarButton[][] = [
  [
    { id: 'bold', label: 'Bold', icon: 'format_bold', action: MarkdownToolbarAction.Bold, shortcut: 'Ctrl+B' },
    { id: 'italic', label: 'Italic', icon: 'format_italic', action: MarkdownToolbarAction.Italic, shortcut: 'Ctrl+I' },
    { id: 'strikethrough', label: 'Strikethrough', icon: 'strikethrough_s', action: MarkdownToolbarAction.Strikethrough }
  ],
  [
    { id: 'heading1', label: 'Heading 1', icon: 'looks_one', action: MarkdownToolbarAction.Heading1 },
    { id: 'heading2', label: 'Heading 2', icon: 'looks_two', action: MarkdownToolbarAction.Heading2 },
    { id: 'heading3', label: 'Heading 3', icon: 'looks_3', action: MarkdownToolbarAction.Heading3 }
  ],
  [
    { id: 'link', label: 'Link', icon: 'link', action: MarkdownToolbarAction.Link, shortcut: 'Ctrl+K' },
    { id: 'image', label: 'Image', icon: 'image', action: MarkdownToolbarAction.Image },
    { id: 'code', label: 'Code', icon: 'code', action: MarkdownToolbarAction.Code },
    { id: 'codeblock', label: 'Code Block', icon: 'code_blocks', action: MarkdownToolbarAction.CodeBlock }
  ],
  [
    { id: 'quote', label: 'Quote', icon: 'format_quote', action: MarkdownToolbarAction.Quote },
    { id: 'ul', label: 'Unordered List', icon: 'format_list_bulleted', action: MarkdownToolbarAction.UnorderedList },
    { id: 'ol', label: 'Ordered List', icon: 'format_list_numbered', action: MarkdownToolbarAction.OrderedList },
    { id: 'table', label: 'Table', icon: 'table_chart', action: MarkdownToolbarAction.Table }
  ],
  [
    { id: 'hr', label: 'Horizontal Rule', icon: 'horizontal_rule', action: MarkdownToolbarAction.HorizontalRule }
  ],
  [
    { id: 'undo', label: 'Undo', icon: 'undo', action: MarkdownToolbarAction.Undo, shortcut: 'Ctrl+Z' },
    { id: 'redo', label: 'Redo', icon: 'redo', action: MarkdownToolbarAction.Redo, shortcut: 'Ctrl+Y' }
  ],
  [
    { id: 'preview', label: 'Preview', icon: 'visibility', action: MarkdownToolbarAction.Preview },
    { id: 'fullscreen', label: 'Fullscreen', icon: 'fullscreen', action: MarkdownToolbarAction.Fullscreen }
  ]
];

/**
 * Wiki index view state
 */
export interface WikiIndexState {
  articles: WikiArticle[];
  categories: WikiCategory[];
  loading: boolean;
  searchFilters: WikiSearchFilters;
  sortOption: string;
  totalCount: number;
}

/**
 * Wiki article form data
 */
export interface WikiArticleFormData {
  title: string;
  content: string;
  categoryId: number;
  tags: string[];
  status: WikiArticleStatus;
  changesSummary?: string;
}
