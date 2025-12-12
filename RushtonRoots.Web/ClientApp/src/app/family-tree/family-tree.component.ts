import { Component } from '@angular/core';

interface Person {
  id: number;
  name: string;
  generation: number;
  parentCouple?: number;
}

interface Couple {
  id: number;
  person1: Person;
  person2: Person;
  generation: number;
}

@Component({
  selector: 'app-family-tree',
  standalone: false,
  templateUrl: './family-tree.component.html',
  styleUrl: './family-tree.component.css'
})
export class FamilyTreeComponent {
  // Root couple (Generation 0)
  rootCouple: Couple = {
    id: 1,
    person1: { id: 1, name: 'John Rushton', generation: 0 },
    person2: { id: 2, name: 'Mary Rushton', generation: 0 },
    generation: 0
  };

  // 7 children with partners (Generation 1)
  childrenCouples: Couple[] = [
    {
      id: 2,
      person1: { id: 3, name: 'Alice', generation: 1, parentCouple: 1 },
      person2: { id: 4, name: 'Tom', generation: 1 },
      generation: 1
    },
    {
      id: 3,
      person1: { id: 5, name: 'Bob', generation: 1, parentCouple: 1 },
      person2: { id: 6, name: 'Sarah', generation: 1 },
      generation: 1
    },
    {
      id: 4,
      person1: { id: 7, name: 'Carol', generation: 1, parentCouple: 1 },
      person2: { id: 8, name: 'James', generation: 1 },
      generation: 1
    },
    {
      id: 5,
      person1: { id: 9, name: 'David', generation: 1, parentCouple: 1 },
      person2: { id: 10, name: 'Emma', generation: 1 },
      generation: 1
    },
    {
      id: 6,
      person1: { id: 11, name: 'Eve', generation: 1, parentCouple: 1 },
      person2: { id: 12, name: 'Michael', generation: 1 },
      generation: 1
    },
    {
      id: 7,
      person1: { id: 13, name: 'Frank', generation: 1, parentCouple: 1 },
      person2: { id: 14, name: 'Lisa', generation: 1 },
      generation: 1
    },
    {
      id: 8,
      person1: { id: 15, name: 'Grace', generation: 1, parentCouple: 1 },
      person2: { id: 16, name: 'Peter', generation: 1 },
      generation: 1
    }
  ];

  // 2nd eldest child (Bob & Sarah) has 2 children
  secondChildGrandchildren: Person[] = [
    { id: 17, name: 'Oliver', generation: 2, parentCouple: 3 },
    { id: 18, name: 'Sophie', generation: 2, parentCouple: 3 }
  ];

  // 3rd eldest child (Carol & James) has 3 children
  thirdChildGrandchildren: Person[] = [
    { id: 19, name: 'Lucas', generation: 2, parentCouple: 4 },
    { id: 20, name: 'Isabella', generation: 2, parentCouple: 4 },
    { id: 21, name: 'Noah', generation: 2, parentCouple: 4 }
  ];

  // 4th eldest child (David & Emma) has 2 children
  fourthChildGrandchildren: Person[] = [
    { id: 22, name: 'Ethan', generation: 2, parentCouple: 5 },
    { id: 23, name: 'Mia', generation: 2, parentCouple: 5 }
  ];
}
