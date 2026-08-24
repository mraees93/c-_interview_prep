import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CaseSearchComponent } from '../components/case-search/case-search.component'

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CaseSearchComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('frontend-app');
}
