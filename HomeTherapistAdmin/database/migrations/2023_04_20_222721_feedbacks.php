<?php

use App\Models\Order;
use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    /**
     * Run the migrations.
     */
    public function up(): void
    {
        Schema::create('feedbacks', function (Blueprint $table) {
            $table->id();
            $table->foreignIdFor(Order::class)->constrained()->cascadeOnUpdate()->cascadeOnDelete();
            $table->string('user_id');
            $table->foreign('user_id')->references('staff_id')->on('users')->cascadeOnUpdate()->restrictOnDelete();
            $table->string('customer_id');
            $table->text('comments')->nullable();
            $table->integer('rating')->unsigned();
            $table->timestamps();
        });

    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::table('feedbacks', function (Blueprint $table) {
            $table->dropForeign(['order_id']);
            $table->dropForeign(['start_dt']);
        });
        Schema::dropIfExists('feedbacks');
    }
};
