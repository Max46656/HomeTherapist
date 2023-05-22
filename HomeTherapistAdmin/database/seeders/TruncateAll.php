<?php

namespace Database\Seeders;

use App\Helpers\DatabaseHelper;
use App\Models\Appointment;
use App\Models\AppointmentDetail;
use App\Models\Article;
use App\Models\Feedback;
use App\Models\Order;
use App\Models\OrderDetail;
use App\Models\Service;
use App\Models\TherapistOpenServices;
use App\Models\TherapistOpenTime;
use App\Models\User;
use Illuminate\Database\Seeder;

class TruncateAll extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        DatabaseHelper::disableForeignKeyConstraints();
        DatabaseHelper::truncateIfExists(Article::class);
        DatabaseHelper::truncateIfExists(Feedback::class);
        DatabaseHelper::truncateIfExists(OrderDetail::class);
        DatabaseHelper::truncateIfExists(Order::class);
        DatabaseHelper::truncateIfExists(AppointmentDetail::class);
        DatabaseHelper::truncateIfExists(Appointment::class);
        DatabaseHelper::truncateIfExists(TherapistOpenServices::class);
        DatabaseHelper::truncateIfExists(TherapistOpenTime::class);
        DatabaseHelper::truncateIfExists(Service::class);
        DatabaseHelper::truncateIfExists(User::class);
        DatabaseHelper::enableForeignKeyConstraints();
    }

}
