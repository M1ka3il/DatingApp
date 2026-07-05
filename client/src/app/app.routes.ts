import { Routes } from '@angular/router';
import { MemberList } from '../members/member-list/member-list';
import { MemberDetail } from '../members/member-detail/member-detail';
import { NotFound } from '../errors/not-found/not-found';
import { ServerError } from '../errors/server-error/server-error';

export const routes: Routes = [
  { path: '', redirectTo: 'members', pathMatch: 'full' },
  { path: 'members', component: MemberList },
  { path: 'members/:id', component: MemberDetail },
  { path: 'not-found', component: NotFound },
  { path: 'server-error', component: ServerError },
  { path: '**', component: NotFound },
];
