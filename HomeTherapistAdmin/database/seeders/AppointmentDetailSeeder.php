<?php

namespace Database\Seeders;

use App\Models\AppointmentDetail;
use Illuminate\Database\Seeder;

class AppointmentDetailSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        $totalRecords = 10000;
        $batchSize = 1000;
        $totalBatches = ceil($totalRecords / $batchSize);
        for ($i = 0; $i < $totalBatches; $i++) {
            AppointmentDetail::factory()->count($batchSize)->create();
            sleep(0.1);
        }
    }
}
