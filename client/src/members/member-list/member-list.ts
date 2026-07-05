import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MemberService } from '../../core/services/member';
import { Member } from '../../core/models/member';

@Component({
  selector: 'app-member-list',
  imports: [RouterLink],
  templateUrl: './member-list.html',
  styleUrl: './member-list.css',
})
export class MemberList implements OnInit {
  private memberService = inject(MemberService);
  protected members = signal<Member[]>([]);

  ngOnInit() {
    this.memberService.getMembers().subscribe({
      next: (members) => this.members.set(members),
    });
  }
}
