import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ApiConsumptionComponent } from '../components/api-consumption/api-consumption.component'

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ApiConsumptionComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('frontend-app');
}
