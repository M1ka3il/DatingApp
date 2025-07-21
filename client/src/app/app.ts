import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.css'
})


export class App implements OnInit{
  private http = inject(HttpClient);
  protected title = 'Dating App';
  protected AppUser = signal<any>([]);;
  
  ngOnInit(): void {
    this.http.get('https://localhost:5001/api/members').subscribe({
      next: response =>  this.AppUser.set(response),
      error: error =>  console.log(error),
      complete: () => { console.log('Request completed')} 
    })
  }


}
