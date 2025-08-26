import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

interface PaginatedList<T> {
  items: T[];
  total: number;
  pageSize: number;
  page: number;
  hasNextPage: boolean;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  public forecasts: WeatherForecast[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getForecasts();
  }

  getForecasts() {
    this.http.get<PaginatedList<WeatherForecast>>('/api/weather-forecasts').subscribe(
      (result) => {
        this.forecasts = result.items;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  title = 'NetCoreAngularApp.Template.Client';
}
