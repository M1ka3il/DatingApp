import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { lastValueFrom } from 'rxjs';
import { NavComponent } from "../layout/nav/nav.component";

@Component({
  selector: 'app-root',
  imports: [NavComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})


export class App implements OnInit{
  private http = inject(HttpClient);
  protected title = 'Dating App';
  protected AppUser = signal<any>([]);;
  
  async ngOnInit() {
    this.AppUser.set(await this.getMembers());
  }

async getMembers(){
  try{
    return lastValueFrom(this.http.get('https://localhost:5001/api/members'));
  } catch (error) {
    console.error('Error fetching members:', error);
    throw error;
  }
}
}
