import { Component, OnInit, Input, ViewEncapsulation } from '@angular/core';
import { marked } from 'marked';
import { WikiArticle, TocEntry } from '../../models/wiki.model';
import { BreadcrumbItem } from '../../../shared/components/breadcrumb/breadcrumb.component';

/**
 * WikiArticleComponent - Displays a wiki article with table of contents
 * 
 * Features:
 * - Renders markdown content as HTML
 * - Generates table of contents from headings
 * - Sticky table of contents navigation
 * - Article metadata display
 * - Print-friendly view
 * - Version information
 */
@Component({
  selector: 'app-wiki-article',
  templateUrl: './wiki-article.component.html',
  styleUrls: ['./wiki-article.component.scss'],
  encapsulation: ViewEncapsulation.None,
  standalone: false
})
export class WikiArticleComponent implements OnInit {
  @Input() articleId?: number;
  @Input() articleSlug?: string;
  
  article: WikiArticle | null = null;
  renderedContent = '';
  tableOfContents: TocEntry[] = [];
  loading = false;
  showToc = true;
  breadcrumbs: BreadcrumbItem[] = [];
  
  // Sample article for demonstration
  private sampleArticle: WikiArticle = {
    id: 1,
    title: 'Getting Started with Genealogy Research',
    slug: 'getting-started-genealogy',
    content: `# Getting Started with Genealogy Research

## Introduction

Genealogy research is the study of family history and lineage. Whether you're just starting out or have been researching for years, this guide will help you build a comprehensive family tree.

## Essential Steps for Beginners

### 1. Start with What You Know

Begin your research with yourself and work backwards. Document:

- Your full name, birth date, and birthplace
- Your parents' names and information
- Your grandparents' details
- Any stories or information passed down through the family

### 2. Gather Family Documents

Collect all available family documents, including:

- Birth certificates
- Marriage certificates
- Death certificates
- Census records
- Military records
- Immigration documents
- Family bibles and letters

### 3. Interview Family Members

Talk to your oldest living relatives. They can provide:

- Personal memories and stories
- Names and dates
- Family connections
- Historical context
- Photos and documents

## Understanding Source Types

### Primary Sources

Primary sources are original documents created at the time of an event:

- Birth, marriage, and death certificates
- Census records
- Church records
- Military service records

### Secondary Sources

Secondary sources are created after the fact:

- Family histories
- Published genealogies
- Some online databases

## Best Practices

1. **Always cite your sources** - Document where you found each piece of information
2. **Verify information** - Cross-reference multiple sources when possible
3. **Organize as you go** - Keep detailed notes and file documents systematically
4. **Back up your data** - Regularly backup your research and family tree
5. **Share carefully** - Respect living people's privacy

## Common Pitfalls to Avoid

- Don't assume information is correct without verification
- Avoid mixing up people with the same name
- Don't ignore female ancestors (maiden names are crucial)
- Don't forget to document negative results

## Next Steps

Once you've mastered the basics:

1. Learn about different record types
2. Understand census records
3. Explore DNA testing options
4. Join genealogy societies
5. Visit archives and libraries

## Resources

- **RushtonRoots Wiki** - Browse our knowledge base
- **National Archives** - Access historical records
- **Local Libraries** - Many have genealogy sections
- **Online Databases** - Ancestry, FamilySearch, etc.

## Conclusion

Genealogy research is a rewarding journey that connects you to your past. Take your time, be thorough, and enjoy discovering your family's story.

---

*Last updated: December 15, 2024*`,
    categoryId: 2,
    categoryName: 'Research Tips',
    authorId: '1',
    authorName: 'John Smith',
    createdDate: new Date('2024-01-15'),
    updatedDate: new Date('2024-12-15'),
    publishedDate: new Date('2024-01-20'),
    status: 'published' as any,
    version: 5,
    viewCount: 234,
    tags: ['beginner', 'research', 'getting-started'],
    isLocked: false
  };
  
  ngOnInit(): void {
    this.loadArticle();
  }
  
  private loadArticle(): void {
    this.loading = true;
    
    // In a real app, this would be an HTTP request based on articleId or articleSlug
    setTimeout(() => {
      this.article = this.sampleArticle;
      if (this.article) {
        this.renderContent();
        this.generateTableOfContents();
        this.updateBreadcrumbs();
      }
      this.loading = false;
    }, 500);
  }
  
  private updateBreadcrumbs(): void {
    if (!this.article) return;
    
    this.breadcrumbs = [
      { label: 'Home', url: '/', icon: 'home' },
      { label: 'Wiki', url: '/Wiki', icon: 'library_books' },
      { label: this.article.categoryName || 'Uncategorized', url: `/Wiki?category=${this.article.categoryId}` },
      { label: this.article.title }
    ];
  }
  
  private renderContent(): void {
    if (!this.article) return;
    
    // Configure marked options
    marked.setOptions({
      gfm: true, // GitHub Flavored Markdown
      breaks: true // Convert line breaks to <br>
    });
    
    // Render markdown to HTML
    this.renderedContent = marked.parse(this.article.content) as string;
  }
  
  private generateTableOfContents(): void {
    if (!this.article) return;
    
    const headingRegex = /^(#{1,6})\s+(.+)$/gm;
    const toc: TocEntry[] = [];
    let match;
    let headingCounter = 0;
    
    while ((match = headingRegex.exec(this.article.content)) !== null) {
      const level = match[1].length;
      const title = match[2].trim();
      const anchor = this.createAnchor(title, headingCounter++);
      
      toc.push({
        id: anchor,
        level,
        title,
        anchor
      });
    }
    
    this.tableOfContents = this.buildHierarchicalToc(toc);
  }
  
  private buildHierarchicalToc(flatToc: TocEntry[]): TocEntry[] {
    if (flatToc.length === 0) return [];
    
    const hierarchical: TocEntry[] = [];
    const stack: TocEntry[] = [];
    
    flatToc.forEach(entry => {
      const newEntry = { ...entry, children: [] };
      
      // Find the appropriate parent
      while (stack.length > 0 && stack[stack.length - 1].level >= newEntry.level) {
        stack.pop();
      }
      
      if (stack.length === 0) {
        hierarchical.push(newEntry);
      } else {
        const parent = stack[stack.length - 1];
        if (!parent.children) parent.children = [];
        parent.children.push(newEntry);
      }
      
      stack.push(newEntry);
    });
    
    return hierarchical;
  }
  
  private createAnchor(title: string, index: number): string {
    // Create URL-friendly anchor from title
    const anchor = title
      .toLowerCase()
      .replace(/[^\w\s-]/g, '')
      .replace(/\s+/g, '-')
      .replace(/-+/g, '-')
      .trim();
    return `${anchor}-${index}`;
  }
  
  scrollToSection(anchor: string): void {
    const element = document.getElementById(anchor);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }
  
  toggleToc(): void {
    this.showToc = !this.showToc;
  }
  
  onPrint(): void {
    window.print();
  }
  
  onEdit(): void {
    console.log('Edit article:', this.article?.slug);
    // Navigate to edit page
  }
  
  onViewHistory(): void {
    console.log('View version history:', this.article?.id);
    // Navigate to version history page or open dialog
  }
  
  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }
}
