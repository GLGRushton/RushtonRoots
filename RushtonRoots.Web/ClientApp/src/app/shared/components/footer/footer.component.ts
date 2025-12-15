import { Component } from '@angular/core';

/**
 * FooterComponent - Application footer with links, social media, and contact info
 * 
 * Features:
 * - Organized footer content sections
 * - Social media links
 * - Contact information
 * - Responsive design
 * - Smooth animations
 * 
 * Usage:
 * <app-footer></app-footer>
 */
@Component({
  selector: 'app-footer',
  standalone: false,
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent {
  currentYear = new Date().getFullYear();

  // Social media links (placeholders for future use)
  socialLinks = [
    { name: 'Facebook', url: '#', icon: 'facebook' },
    { name: 'Twitter', url: '#', icon: 'twitter' },
    { name: 'Instagram', url: '#', icon: 'instagram' },
    { name: 'LinkedIn', url: '#', icon: 'linkedin' }
  ];

  // Footer navigation links
  footerLinks = {
    about: [
      { label: 'About Us', url: '/about' },
      { label: 'Our Story', url: '/story' },
      { label: 'Mission', url: '/mission' }
    ],
    resources: [
      { label: 'Wiki', url: '/Wiki/Index' },
      { label: 'Recipes', url: '/Recipe/Index' },
      { label: 'Traditions', url: '/Tradition/Index' },
      { label: 'Stories', url: '/StoryView/Index' }
    ],
    support: [
      { label: 'Help Center', url: '/help' },
      { label: 'Contact Us', url: '/contact' },
      { label: 'Privacy Policy', url: '/privacy' },
      { label: 'Terms of Service', url: '/terms' }
    ]
  };

  // Contact information
  contactInfo = {
    email: 'info@rushtonroots.com',
    phone: '(555) 123-4567'
  };

  navigateTo(url: string): void {
    window.location.href = url;
  }
}
