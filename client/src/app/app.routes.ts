import { Routes } from '@angular/router';
import { MemberList } from '../members/member-list/member-list';
import { MemberDetail } from '../members/member-detail/member-detail';

export const routes: Routes = [
  { path: '', redirectTo: 'members', pathMatch: 'full' },
  { path: 'members', component: MemberList },
  { path: 'members/:id', component: MemberDetail },
];
