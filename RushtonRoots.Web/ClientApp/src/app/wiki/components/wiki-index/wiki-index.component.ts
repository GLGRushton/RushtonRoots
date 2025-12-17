import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { debounceTime, takeUntil } from 'rxjs/operators';
import { WikiArticle, WikiCategory, WikiSearchFilters, WikiSortOption, WIKI_SORT_OPTIONS, WikiArticleStatus } from '../../models/wiki.model';
import { BreadcrumbItem } from '../../../shared/components/breadcrumb/breadcrumb.component';

/**
 * WikiIndexComponent - Main wiki navigation and article listing
 * 
 * Features:
 * - Grid/List view of wiki articles
 * - Category filtering
 * - Search functionality
 * - Sort options
 * - Article status filtering
 * - Create new article button
 * - Responsive design
 */
@Component({
  selector: 'app-wiki-index',
  templateUrl: './wiki-index.component.html',
  styleUrls: ['./wiki-index.component.scss'],
  standalone: false
})
export class WikiIndexComponent implements OnInit, OnDestroy {
  articles: WikiArticle[] = [];
  filteredArticles: WikiArticle[] = [];
  categories: WikiCategory[] = [];
  loading = false;
  
  searchFilters: WikiSearchFilters = {
    query: ''
  };
  
  sortOptions: WikiSortOption[] = WIKI_SORT_OPTIONS;
  selectedSort = 'updated-desc';
  
  viewMode: 'grid' | 'list' = 'grid';
  
  // Breadcrumb navigation
  breadcrumbs: BreadcrumbItem[] = [
    { label: 'Home', url: '/', icon: 'home' },
    { label: 'Wiki', icon: 'library_books' }
  ];
  
  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();
  
  // Sample data for demonstration
  private sampleCategories: WikiCategory[] = [
    { id: 1, name: 'Family History', slug: 'family-history', description: 'Articles about family history and genealogy', articleCount: 12, order: 1, icon: 'family_restroom', color: '#2e7d32' },
    { id: 2, name: 'Research Tips', slug: 'research-tips', description: 'Tips and guides for genealogical research', articleCount: 8, order: 2, icon: 'search', color: '#1976d2' },
    { id: 3, name: 'Record Types', slug: 'record-types', description: 'Information about different types of records', articleCount: 15, order: 3, icon: 'description', color: '#f57c00' },
    { id: 4, name: 'How-To Guides', slug: 'how-to-guides', description: 'Step-by-step guides for using RushtonRoots', articleCount: 6, order: 4, icon: 'help', color: '#7b1fa2' },
    { id: 5, name: 'Best Practices', slug: 'best-practices', description: 'Best practices for family tree management', articleCount: 10, order: 5, icon: 'verified', color: '#c62828' }
  ];
  
  private sampleArticles: WikiArticle[] = [
    {
      id: 1,
      title: 'Getting Started with Genealogy Research',
      slug: 'getting-started-genealogy',
      content: '# Getting Started with Genealogy Research\n\nLearn the basics of genealogy research...',
      categoryId: 2,
      categoryName: 'Research Tips',
      authorId: '1',
      authorName: 'John Smith',
      createdDate: new Date('2024-01-15'),
      updatedDate: new Date('2024-12-10'),
      publishedDate: new Date('2024-01-20'),
      status: WikiArticleStatus.Published,
      version: 5,
      viewCount: 234,
      tags: ['beginner', 'research', 'getting-started'],
      isLocked: false
    },
    {
      id: 2,
      title: 'Understanding Census Records',
      slug: 'understanding-census-records',
      content: '# Understanding Census Records\n\nCensus records are invaluable resources...',
      categoryId: 3,
      categoryName: 'Record Types',
      authorId: '2',
      authorName: 'Jane Doe',
      createdDate: new Date('2024-02-10'),
      updatedDate: new Date('2024-11-28'),
      publishedDate: new Date('2024-02-15'),
      status: WikiArticleStatus.Published,
      version: 3,
      viewCount: 189,
      tags: ['census', 'records', 'research'],
      isLocked: false
    },
    {
      id: 3,
      title: 'How to Add a Person to Your Family Tree',
      slug: 'how-to-add-person',
      content: '# How to Add a Person to Your Family Tree\n\nFollow these steps...',
      categoryId: 4,
      categoryName: 'How-To Guides',
      authorId: '1',
      authorName: 'John Smith',
      createdDate: new Date('2024-03-05'),
      updatedDate: new Date('2024-12-05'),
      publishedDate: new Date('2024-03-10'),
      status: WikiArticleStatus.Published,
      version: 7,
      viewCount: 456,
      tags: ['how-to', 'person', 'tutorial'],
      isLocked: false
    },
    {
      id: 4,
      title: 'Best Practices for Source Citations',
      slug: 'source-citation-best-practices',
      content: '# Best Practices for Source Citations\n\nProper source citations are crucial...',
      categoryId: 5,
      categoryName: 'Best Practices',
      authorId: '3',
      authorName: 'Mary Johnson',
      createdDate: new Date('2024-04-12'),
      updatedDate: new Date('2024-12-01'),
      publishedDate: new Date('2024-04-15'),
      status: WikiArticleStatus.Published,
      version: 4,
      viewCount: 178,
      tags: ['sources', 'citations', 'best-practices'],
      isLocked: false
    },
    {
      id: 5,
      title: 'The Rushton Family History',
      slug: 'rushton-family-history',
      content: '# The Rushton Family History\n\nThe Rushton family originated in...',
      categoryId: 1,
      categoryName: 'Family History',
      authorId: '1',
      authorName: 'John Smith',
      createdDate: new Date('2024-05-20'),
      updatedDate: new Date('2024-12-15'),
      publishedDate: new Date('2024-05-25'),
      status: WikiArticleStatus.Published,
      version: 12,
      viewCount: 892,
      tags: ['rushton', 'family-history', 'origins'],
      isLocked: true,
      lockedBy: 'Admin',
      lockedUntil: new Date('2024-12-20')
    },
    {
      id: 6,
      title: 'Draft: Understanding DNA Testing',
      slug: 'understanding-dna-testing',
      content: '# Understanding DNA Testing\n\nWork in progress...',
      categoryId: 2,
      categoryName: 'Research Tips',
      authorId: '2',
      authorName: 'Jane Doe',
      createdDate: new Date('2024-12-01'),
      updatedDate: new Date('2024-12-14'),
      status: WikiArticleStatus.Draft,
      version: 1,
      viewCount: 12,
      tags: ['dna', 'testing', 'draft'],
      isLocked: false
    }
  ];
  
  ngOnInit(): void {
    this.loadCategories();
    this.loadArticles();
    this.setupSearch();
  }
  
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
  
  private setupSearch(): void {
    this.searchSubject
      .pipe(
        debounceTime(300),
        takeUntil(this.destroy$)
      )
      .subscribe(() => {
        this.applyFilters();
      });
  }
  
  private loadCategories(): void {
    // In a real app, this would be an HTTP request
    this.categories = [...this.sampleCategories];
  }
  
  private loadArticles(): void {
    this.loading = true;
    
    // In a real app, this would be an HTTP request
    setTimeout(() => {
      this.articles = [...this.sampleArticles];
      this.applyFilters();
      this.loading = false;
    }, 500);
  }
  
  onSearchChange(query: string): void {
    this.searchFilters.query = query;
    this.searchSubject.next(query);
  }
  
  onCategoryChange(categoryId: number | undefined): void {
    this.searchFilters.categoryId = categoryId;
    this.applyFilters();
  }
  
  onStatusChange(status: WikiArticleStatus | undefined): void {
    this.searchFilters.status = status;
    this.applyFilters();
  }
  
  onSortChange(sortOption: string): void {
    this.selectedSort = sortOption;
    this.applyFilters();
  }
  
  toggleViewMode(): void {
    this.viewMode = this.viewMode === 'grid' ? 'list' : 'grid';
  }
  
  private applyFilters(): void {
    let filtered = [...this.articles];
    
    // Apply search filter
    if (this.searchFilters.query) {
      const query = this.searchFilters.query.toLowerCase();
      filtered = filtered.filter(article =>
        article.title.toLowerCase().includes(query) ||
        article.content.toLowerCase().includes(query) ||
        article.tags.some(tag => tag.toLowerCase().includes(query))
      );
    }
    
    // Apply category filter
    if (this.searchFilters.categoryId) {
      filtered = filtered.filter(article => article.categoryId === this.searchFilters.categoryId);
    }
    
    // Apply status filter
    if (this.searchFilters.status) {
      filtered = filtered.filter(article => article.status === this.searchFilters.status);
    }
    
    // Apply sorting
    filtered = this.sortArticles(filtered, this.selectedSort);
    
    this.filteredArticles = filtered;
  }
  
  private sortArticles(articles: WikiArticle[], sortOption: string): WikiArticle[] {
    const sorted = [...articles];
    
    switch (sortOption) {
      case 'title-asc':
        return sorted.sort((a, b) => a.title.localeCompare(b.title));
      case 'title-desc':
        return sorted.sort((a, b) => b.title.localeCompare(a.title));
      case 'updated-desc':
        return sorted.sort((a, b) => new Date(b.updatedDate).getTime() - new Date(a.updatedDate).getTime());
      case 'updated-asc':
        return sorted.sort((a, b) => new Date(a.updatedDate).getTime() - new Date(b.updatedDate).getTime());
      case 'created-desc':
        return sorted.sort((a, b) => new Date(b.createdDate).getTime() - new Date(a.createdDate).getTime());
      case 'created-asc':
        return sorted.sort((a, b) => new Date(a.createdDate).getTime() - new Date(b.createdDate).getTime());
      case 'views-desc':
        return sorted.sort((a, b) => b.viewCount - a.viewCount);
      case 'views-asc':
        return sorted.sort((a, b) => a.viewCount - b.viewCount);
      default:
        return sorted;
    }
  }
  
  onCreateArticle(): void {
    console.log('Create new article');
    // Navigate to create article page or open dialog
  }
  
  onViewArticle(article: WikiArticle): void {
    console.log('View article:', article.slug);
    // Navigate to article detail page
  }
  
  onEditArticle(article: WikiArticle): void {
    console.log('Edit article:', article.slug);
    // Navigate to edit article page
  }
  
  onDeleteArticle(article: WikiArticle): void {
    console.log('Delete article:', article.slug);
    // Show confirmation dialog and delete
  }
  
  getStatusColor(status: WikiArticleStatus): string {
    switch (status) {
      case WikiArticleStatus.Published:
        return 'accent';
      case WikiArticleStatus.Draft:
        return 'warn';
      case WikiArticleStatus.Archived:
        return '';
      default:
        return '';
    }
  }
  
  getCategoryById(id: number): WikiCategory | undefined {
    return this.categories.find(c => c.id === id);
  }
  
  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString();
  }
}
