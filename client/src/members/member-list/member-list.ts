import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MemberService } from '../../core/services/member';
import { Member } from '../../core/models/member';
import { Pagination } from '../../core/models/pagination';
import { UserParams } from '../../core/models/user-params';

@Component({
  selector: 'app-member-list',
  imports: [RouterLink, FormsModule],
  templateUrl: './member-list.html',
  styleUrl: './member-list.css',
})
export class MemberList implements OnInit {
  private memberService = inject(MemberService);
  protected members = signal<Member[]>([]);
  protected pagination = signal<Pagination | undefined>(undefined);
  protected userParams = new UserParams();

  ngOnInit() {
    this.loadMembers();
  }

  loadMembers() {
    this.memberService.getMembers(this.userParams).subscribe({
      next: (result) => {
        this.members.set(result.items ?? []);
        this.pagination.set(result.pagination);
      },
    });
  }

  search() {
    this.userParams.pageNumber = 1;
    this.loadMembers();
  }

  resetFilters() {
    this.userParams = new UserParams();
    this.loadMembers();
  }

  setDirection(direction: 'asc' | 'desc') {
    this.userParams.direction = direction;
    this.search();
  }

  changePage(page: number) {
    const totalPages = this.pagination()?.totalPages ?? 1;
    if (page < 1 || page > totalPages || page === this.userParams.pageNumber) return;
    this.userParams.pageNumber = page;
    this.loadMembers();
  }
}
