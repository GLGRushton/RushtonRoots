import { Injectable } from '@angular/core';
import axe, { AxeResults, Result } from 'axe-core';

/**
 * Service for automated accessibility testing using Axe
 */
@Injectable({
  providedIn: 'root'
})
export class AccessibilityTestingService {
  /**
   * Run accessibility audit on the entire document
   */
  async runAudit(): Promise<AxeResults> {
    return await axe.run(document);
  }

  /**
   * Run accessibility audit on a specific element
   */
  async runAuditOnElement(element: HTMLElement): Promise<AxeResults> {
    return await axe.run(element);
  }

  /**
   * Get formatted violations from audit results
   */
  getViolations(results: AxeResults): Result[] {
    return results.violations;
  }

  /**
   * Get formatted passes from audit results
   */
  getPasses(results: AxeResults): Result[] {
    return results.passes;
  }

  /**
   * Log audit results to console
   */
  logResults(results: AxeResults): void {
    const violations = this.getViolations(results);
    const passes = this.getPasses(results);

    console.group('🔍 Accessibility Audit Results');
    console.log(`✅ Passed: ${passes.length} rules`);
    console.log(`❌ Violations: ${violations.length} rules`);
    
    if (violations.length > 0) {
      console.group('❌ Violations');
      violations.forEach((violation: Result) => {
        console.group(`${violation.impact?.toUpperCase()} - ${violation.help}`);
        console.log('Description:', violation.description);
        console.log('Help URL:', violation.helpUrl);
        console.log('Affected nodes:', violation.nodes.length);
        violation.nodes.forEach((node: any, index: number) => {
          console.log(`  ${index + 1}. ${node.html}`);
          console.log(`     Selector: ${node.target.join(', ')}`);
        });
        console.groupEnd();
      });
      console.groupEnd();
    }
    
    console.groupEnd();
  }

  /**
   * Get accessibility score (0-100)
   */
  getScore(results: AxeResults): number {
    const total = results.violations.length + results.passes.length;
    if (total === 0) return 100;
    return Math.round((results.passes.length / total) * 100);
  }

  /**
   * Get violation summary by impact level
   */
  getViolationSummary(results: AxeResults): { critical: number; serious: number; moderate: number; minor: number } {
    const violations = this.getViolations(results);
    return {
      critical: violations.filter((v: Result) => v.impact === 'critical').length,
      serious: violations.filter((v: Result) => v.impact === 'serious').length,
      moderate: violations.filter((v: Result) => v.impact === 'moderate').length,
      minor: violations.filter((v: Result) => v.impact === 'minor').length
    };
  }
}
