<?php

use Carbon\Carbon;
use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    /**
     * Run the migrations.
     */
    public function up(): void
    {
        Schema::create('calendar', function (Blueprint $table) {
            $table->id();
            $table->date('date')->nullable();
            $table->dateTime('dt')->nullable()->index();
            $table->unsignedSmallInteger('year')->nullable();
            $table->unsignedTinyInteger('quarter')->nullable();
            $table->unsignedTinyInteger('month')->nullable();
            $table->unsignedTinyInteger('day')->nullable();
            $table->unsignedTinyInteger('day_of_week')->nullable();
            $table->unsignedTinyInteger('week_of_year')->nullable();
            $table->boolean('is_weekend')->default(false)->nullable();
            $table->boolean('is_holiday')->default(false)->nullable();

            $table->index(['year', 'month', 'day'], 'Ymd');
        });

        $startDate = '2023-01-13 00:00';
        $endDate = '2024-12-31 23:59';
        $batchSize = 1000;

        $currentDate = Carbon::parse($startDate);
        $endDateTime = Carbon::parse($endDate);

        while ($currentDate <= $endDateTime) {
            $dates = [];
            for ($i = 0; $i < $batchSize && $currentDate <= $endDateTime; $i++) {
                $date = $currentDate->format('Y-m-d');
                $dt = $currentDate->format('Y-m-d H:i');
                $year = $currentDate->format('Y');
                $month = $currentDate->format('n');
                $day = $currentDate->format('d');
                $dayOfWeek = $currentDate->dayOfWeek;
                $weekOfYear = $currentDate->weekOfYear;
                $quarter = $currentDate->quarter;
                $isWeekend = $currentDate->isWeekend();

                $dates[] = [
                    'date' => $date,
                    'dt' => $dt,
                    'year' => $year,
                    'month' => $month,
                    'day' => $day,
                    'day_of_week' => $dayOfWeek,
                    'week_of_year' => $weekOfYear,
                    'quarter' => $quarter,
                    'is_weekend' => $isWeekend,
                    'is_holiday' => false,
                ];

                $currentDate->addMinutes(10);
            }

            DB::table('calendar')->insert($dates);
        }

    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('calendar');
    }
};
