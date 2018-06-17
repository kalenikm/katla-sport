import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HiveService } from '../services/hive.service';
import { Hive } from '../models/hive';

@Component({
  selector: 'app-hive-form',
  templateUrl: './hive-form.component.html',
  styleUrls: ['./hive-form.component.css']
})
export class HiveFormComponent implements OnInit {

  hive = new Hive(0, "", "", "", false, "");
  existed = false;
  hiveId: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private hiveService: HiveService
  ) { }

  ngOnInit() {
    this.route.params.subscribe(p => {
      if (p['id'] === undefined) return;
      this.hiveId = p['id'];
      this.hiveService.getHive(this.hiveId).subscribe(h => this.hive = h);
      this.existed = true;
    });
  }

  navigateToHives() {
    this.router.navigate(['/hives']);
  }

  onCancel() {
    this.navigateToHives();
  }
  
  onSubmit() {
    if (this.existed) {
      this.hiveService.updateHive(this.hive).subscribe(h => this.navigateToHives());
    } else {
      this.hive.id = this.hiveId;
      this.hiveService.addHive(this.hive).subscribe(h => this.navigateToHives());
    }
  }

  onDelete() {
    this.hiveService.setHiveStatus(this.hiveId, true).subscribe(h => this.hive.isDeleted = true);
  }

  onUndelete() {
    this.hiveService.setHiveStatus(this.hiveId, false).subscribe(h => this.hive.isDeleted = false);
  }

  onPurge() {
    if (this.hive.isDeleted) {
      this.hiveService.deleteHive(this.hiveId).subscribe(h => this.navigateToHives());
    }
  }
}
